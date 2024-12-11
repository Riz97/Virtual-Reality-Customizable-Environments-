using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class SketchfabPreviewImagesDownloader : MonoBehaviour
{
   
     string imageUrl = SketchfabBrowser.ImagePreview;

    [Header("UI Target")]
    public RawImage targetRawImage; // La RawImage su cui applicare l'immagine scaricata

    [Header("Adattamento")]
    public bool preserveAspect = true; // Mantieni le proporzioni dell'immagine

    void Start()
    {
        // Avvia il download e applica l'immagine automaticamente
        StartCoroutine(DownloadAndApplyImage(imageUrl));
    }

    IEnumerator DownloadAndApplyImage(string url)
    {
        // Avvia il download dell'immagine
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
          

            // Aspetta il completamento della richiesta
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Log di errore
                Debug.LogError($"Errore durante il download: {webRequest.error}");
            }
            else
            {
                // Ottieni la texture
                Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(webRequest);

                if (downloadedTexture != null)
                {
                    

                    // Assegna la texture alla RawImage
                    if (targetRawImage != null)
                    {
                        targetRawImage.texture = downloadedTexture;

                        // Adatta l'immagine alla forma della RawImage
                        if (preserveAspect)
                        {
                            AdjustAspectRatio(targetRawImage, downloadedTexture);
                        }
                    }
                }
              
            }
        }
    }

    void AdjustAspectRatio(RawImage rawImage, Texture2D texture)
    {
        // Ottieni il RectTransform della RawImage
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();

        // Calcola il rapporto tra larghezza e altezza
        float textureAspectRatio = (float)texture.width / texture.height;
        float rawImageAspectRatio = rectTransform.rect.width / rectTransform.rect.height;

        if (textureAspectRatio > rawImageAspectRatio)
        {
            // L'immagine è più larga: ridimensiona l'altezza
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width / textureAspectRatio);
        }
        else
        {
            // L'immagine è più alta: ridimensiona la larghezza
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.height * textureAspectRatio, rectTransform.rect.height);
        }
    }
}