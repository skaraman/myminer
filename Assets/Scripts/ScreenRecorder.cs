using UnityEngine;
using System.Collections;
using System.IO;

// Screen Recorder will save individual images of active scene in any resolution and of a specific image format
// including raw, jpg, png, and ppm.  Raw and PPM are the fastest image formats for saving.
//
// You can compile these images into a video using ffmpeg:
// ffmpeg -i screen_3840x2160_%d.ppm -y test.avi

public class ScreenRecorder : MonoBehaviour {
    public TheCube cube;
    // 4k = 3840 x 2160   1080p = 1920 x 1080
    public int captureWidth = 800;
    public int captureHeight = 600;

    // optional game object to hide during screenshots (usually your scene canvas hud)
    public GameObject hideGameObject;

    // optimize for many screenshots will not destroy any objects so future screenshots will be fast
    public bool optimizeForManyScreenshots = true;

    // configure with raw, jpg, png, or ppm (simple raw format), SP = Sprite
    public enum Format { RAW, JPG, PNG, PPM, SP };
    public Format format = Format.SP;

    // folder to write output (defaults to data path)
    public string folder;
    public string publicfilename;

    // private vars for screenshot
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;
    private int counter = 0; // image #

    // commands
    private bool captureScreenshot = false;
    private bool captureVideo = false;

    private bool faded;
    private Color black = new Color(0, 0, 0);

    // create a unique filename using a one-up variable
    private string uniqueFilename (int width, int height) {
        // if folder not specified by now use a good default
        if (folder == null || folder.Length == 0) {
            folder = Application.persistentDataPath;
            if (Application.isEditor) {
                // put screenshots in folder above asset path so unity doesn't index the files
                var stringPath = folder + "/..";
                folder = Path.GetFullPath(stringPath);
            }
            folder += "/screenshots";

            // make sure directoroy exists
            System.IO.Directory.CreateDirectory(folder);

            // count number of files of specified format in folder
            string mask = string.Format("screen_{0}x{1}*.{2}", width, height, format.ToString().ToLower());
            counter = Directory.GetFiles(folder, mask, SearchOption.TopDirectoryOnly).Length;
        }

        // use width, height, and counter for unique file name
        publicfilename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", folder, width, height, counter, format.ToString().ToLower());

        // up counter for next call
        ++counter;

        // return unique filename
        return publicfilename;
    }


    public void CaptureScreenshot (bool fade, string fn) {
        captureScreenshot = true;
        Filename = fn;
        faded = fade;
    }

    public string Filename {
        get {
            return publicfilename;
        }
        set {
            publicfilename = value;
        }
    }

    Texture2D RemoveColor (Color c, Texture2D imgs) {
        Color[] pixels = imgs.GetPixels(0, 0, imgs.width, imgs.height, 0);

        Color newcol = new Color(1.00f, 0.78f, 0.01f, 1);
        Color transp = new Color(1, 1, 1, 0);

        for (int p = 0; p < pixels.Length; p++) {
            if ((pixels[p].r <= 1 &&
                pixels[p].g <= 1 &&
                pixels[p].b <= 1 &&
                pixels[p].r == pixels[p].g &&
                pixels[p].g == pixels[p].b) ||
                (pixels[p].r <= .99f &&
                 pixels[p].g <= .99f &&
                 pixels[p].b <= .99f)) {
                pixels[p] = newcol;
            }
            else {
                pixels[p] = transp;
            }
        }

        imgs.SetPixels(0, 0, imgs.width, imgs.height, pixels, 0);
        imgs.Apply();
        return imgs;
    }

    void Update () {
        // check keyboard 'k' for one time screenshot capture and holding down 'v' for continious screenshots
        captureScreenshot |= Input.GetKeyDown("k");
        captureVideo = Input.GetKey("v");

        if (captureScreenshot || captureVideo) {
            captureScreenshot = false;

            // hide optional game object if set
            if (hideGameObject != null) hideGameObject.SetActive(false);

            // create screenshot objects if needed
            if (renderTexture == null) {
                // creates off-screen render texture that can rendered into
                rect = new Rect(0, 0, captureWidth, captureHeight);
                renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
                renderTexture.filterMode = FilterMode.Point;
                screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.ARGB32, false);
            }

            // get main camera and manually render scene into rt
            Camera camera = this.GetComponent<Camera>(); // NOTE: added because there was no reference to camera in original script; must add this script to Camera
            camera.targetTexture = renderTexture;
            camera.Render();

            // read pixels will read from the currently active render texture so make our offscreen 
            // render texture active and then read the pixels
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);
            RemoveColor(black, screenShot);    //************************************************************
            // reset active camera texture and render texture
            camera.targetTexture = null;
            RenderTexture.active = null;

            // get our unique filename
            string filename = publicfilename != "" ? publicfilename : uniqueFilename((int)rect.width, (int)rect.height);

            // pull in our file header/data bytes for the specified image format (has to be done from main thread)
            byte[] fileHeader = null;
            byte[] fileData = null;
            bool thread = true;
            Sprite sp = null;
            if (format == Format.RAW) {
                fileData = screenShot.GetRawTextureData();
            }
            else if (format == Format.PNG) {
                fileData = screenShot.EncodeToPNG();
            }
            else if (format == Format.JPG) {
                fileData = screenShot.EncodeToJPG();
            }
            else if (format == Format.PPM) // ppm
            {
                // create a file header for ppm formatted file
                string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height);
                fileHeader = System.Text.Encoding.ASCII.GetBytes(headerStr);
                fileData = screenShot.GetRawTextureData();
            }
            else if (format == Format.SP) {
                thread = false; // debugger - normal value is false
                if (thread == true) {
                    fileData = screenShot.EncodeToPNG();
                }
                screenShot.Apply();
                screenShot.filterMode = FilterMode.Point;
                sp = Sprite.Create(screenShot, rect, new Vector2(0, 0));

                cube.PutScreenshot(sp, Filename, screenShot);
            }

            // create new thread to save the image to file (only operation that can be done in background)
            if (thread) {
                new System.Threading.Thread(() => {
                    // create file and write optional header with image bytes
                    var f = System.IO.File.Create(filename);
                    if (fileHeader != null) f.Write(fileHeader, 0, fileHeader.Length);
                    f.Write(fileData, 0, fileData.Length);
                    f.Close();
                    Debug.Log(string.Format("Wrote screenshot {0} of size {1}", filename, fileData.Length));
                }).Start();
            }

            // unhide optional game object if set
            if (hideGameObject != null) hideGameObject.SetActive(true);

            // cleanup if needed
            if (optimizeForManyScreenshots == false) {
                Destroy(renderTexture);
                renderTexture = null;
                screenShot = null;
            }

            // callbacks
            if (faded) {
                cube.FadeCubeCallback(faded);
            }
        }
    }
}