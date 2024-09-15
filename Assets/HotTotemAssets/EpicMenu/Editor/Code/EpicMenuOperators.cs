using UnityEngine;

namespace HotTotemAssets.EpicMenu {
    public static class EpicMenuOperators {
        static Texture bg;
        public static Texture blurredBG;
        public static Rect lastRect;
        static Rect lastUsedRect;
        public static Color tintingColor = Color.black;

        static void getTexture (Rect rect) {
            blurredBG = bg;
        }

        public static void ScreenShot (Rect rect) {
            lastUsedRect = rect;
            int width = (int)rect.width;
            int height = (int)rect.height;
            int x = (int)rect.x;
            int y = (int)rect.y;
            Vector2 position = new Vector2 (x, y);
            Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel (position, width, height);
            Texture2D texture = new Texture2D (width, height);
            texture.SetPixels (pixels);
            texture.Apply ();
            bg = texture;
            getTexture (rect);
        }

        public static void ScreenShot () {
            int width = (int)lastUsedRect.width;
            int height = (int)lastUsedRect.height;
            int x = (int)lastUsedRect.x;
            int y = (int)lastUsedRect.y;
            Vector2 position = new Vector2 (x, y);
            Color[] pixels = UnityEditorInternal.InternalEditorUtility.ReadScreenPixel (position, width, height);
            Texture2D texture = new Texture2D (width, height);
            texture.SetPixels (pixels);
            texture.Apply ();
            bg = texture;
            getTexture (lastUsedRect);
        }

        private class Blur {
            float tinting = 0f;
            float blurSize = 0.1f;
            int passes = 8;

            Material blurMaterial;
            RenderTexture destTexture;

            public Blur (int width, int height, Color tint) {
                blurMaterial = new Material (Shader.Find ("Hidden/Blur"));
                blurMaterial.SetColor ("_Tint", tint);
                blurMaterial.SetFloat ("_Tinting", tinting);
                blurMaterial.SetFloat ("_BlurSize", blurSize);
                destTexture = new RenderTexture (width, height, 0);
                destTexture.Create ();
            }

            public Texture BlurTexture (Texture sourceTexture) {
                RenderTexture active = RenderTexture.active; // Save original RenderTexture so we can restore when we're done.
                try {
                    RenderTexture tempA = RenderTexture.GetTemporary (sourceTexture.width, sourceTexture.height);
                    RenderTexture tempB = RenderTexture.GetTemporary (sourceTexture.width, sourceTexture.height);
                    for (int i = 0; i < passes; i++) {
                        if (i == 0) {
                            Graphics.Blit (sourceTexture, tempA, blurMaterial, 0);
                        } else {
                            Graphics.Blit (tempB, tempA, blurMaterial, 0);
                        }
                        Graphics.Blit (tempA, tempB, blurMaterial, 1);
                    }
                    Graphics.Blit (tempB, destTexture, blurMaterial, 2);
                    RenderTexture.ReleaseTemporary (tempA);
                    RenderTexture.ReleaseTemporary (tempB);
                } catch (System.Exception e) {
                    Debug.LogException (e);
                } finally {
                    RenderTexture.active = active; // Restore
                }
                return destTexture;
            }
        }
    }
}