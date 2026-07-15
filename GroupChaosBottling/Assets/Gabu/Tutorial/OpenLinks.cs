using UnityEngine;

public class OpenLinks : MonoBehaviour
{
    readonly string notebookURL = "https://notebooklm.google.com/notebook/6a7ab319-5e32-4c3d-bb14-8d4729a4d944?_gl=1*45i1yc*_ga*MTIxMjg4NzI0Ny4xNzQ4OTE4MTAy*_ga_W0LDH41ZCB*czE3NDkxNjg4NzgkbzIkZzAkdDE3NDkxNjg4NzgkajYwJGwwJGgw";
    readonly string formsURL = "https://docs.google.com/forms/d/e/1FAIpQLScer-trSvbS2cpOSVJ-AIhtqpaJ4VLfHI3gw7kB5tYFZuNfTQ/viewform?usp=dialog";

    public void OpenNoteBook()
    {
        Application.OpenURL(notebookURL);
    }

    public void OpenForms()
    {
        Application.OpenURL(formsURL);
    }


}
