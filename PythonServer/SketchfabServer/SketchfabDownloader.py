import requests
import os
import socket
from time import sleep
# Il tuo token di accesso Sketchfab
api_token = "vvAlrUNsMbiGG02w7So1CDE61eimIY"  # Authorization Token, follow the SKETCHFAB README to understand how to obtain one  (remember that it expires)
HOST = '127.0.1.10'  # Standard loopback interface address (localhost)
PORT = 12321 
percorso_salvataggio = "C:/Users/ricky/Desktop/file.zip" #path of the zip of the model just downloaded

#TODO - Make this python script a server that receives Sketchfab UID from Unity

#model_uid = "6e35f6fdf1264db4880049b802f18ea2"  # 3D Model ID

def get_download_link(model_uid, api_token):
    #URL of the Model that we want to download
    url = f"https://api.sketchfab.com/v3/models/{model_uid}/download"
    headers = {"Authorization": f"Bearer {api_token}"}

    # Request for getting the JSON
    response = requests.get(url, headers=headers)

    # Check if the request is OK
    if response.status_code == 200:
        #Response saved as json
        response_json = response.json()
        model_link = response_json.get('source', {}).get('url') #First url of the field 'source'
        return model_link
       
    else:
        print(f"Errore durante il recupero del link di download: {response.status_code}")
        return None

print("Welcome to the Sketchfab Server")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST,PORT))
    s.listen()
    conn, addr = s.accept()
    print('Server in ascolto su', HOST, PORT)
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(102400)
            print(data.decode()) #Uid sent by the Unity Framework Download Button

            #Downloading the 3D Model
            download_url = get_download_link(data.decode(), api_token)
            print(download_url)

            response = requests.get(download_url, stream=True)

            #File Saving
            with open(percorso_salvataggio, 'wb') as f:
                for chunk in response.iter_content(chunk_size=1024):
                    if chunk:
                        f.write(chunk)







