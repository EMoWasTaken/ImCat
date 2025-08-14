using System;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Source_Code {
    public class Main : MonoBehaviour {
        public TMP_InputField catField;
        public Image editDisplay;
        
        public async void SelectFile() {
            string path = await CrossPlatformFilePicker.PickFileAsync();
            Debug.Log("Hallo");
            Debug.Log(string.IsNullOrEmpty(path) ? "No file selected" : $"Selected file: {path}");
            
            string id = SqlHandler.Query("SELECT max(ID) + 1 FROM Images;");
            Debug.Log(id);
            SqlHandler.NonQuery(
                $"INSERT INTO Images (ID, FilePath) VALUES (" +
                $"{id}, '{path}');");

            Texture2D editTex = new Texture2D(1, 1);
            if (path != null) editTex.LoadImage(File.ReadAllBytes(path));
            Sprite editImg = Sprite.Create(editTex, new Rect(0.0f, 0.0f, editTex.width, editTex.height),
                new Vector2(0.5f, 0.5f), 100);
            editDisplay.sprite = editImg;
            
            editDisplay.SetNativeSize(); //890, 630

            RectTransform r = editDisplay.GetComponent<RectTransform>();

            while (r.sizeDelta.x > 890 || r.sizeDelta.y > 630)
                r.sizeDelta *= 0.95f;
        }
        
        private void Awake() {
            SqlHandler.DBLocation = "URI=file:ImCatDB.db";
            SqlHandler.InitSqlite();
            SqlHandler.InitDB();
        }
    }
}
