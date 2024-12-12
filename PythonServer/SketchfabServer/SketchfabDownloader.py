import requests
import os
import socket
from time import sleep
import zipfile

# Il tuo token di accesso Sketchfab
api_token = "vvAlrUNsMbiGG02w7So1CDE61eimIY"
HOST = '127.0.1.10'
PORT = 12321 

# Percorso base per il salvataggio dei file
base_save_path = "C:/Users/ricky/Desktop/Framework/Virtual-Reality-Customizable-Environments-/Assets/Imported/"

def get_download_link(model_uid, api_token):
    # URL of the Model that we want to download
    url = f"https://api.sketchfab.com/v3/models/{model_uid}/download"
    headers = {"Authorization": f"Bearer {api_token}"}

    # Request for getting the JSON
    response = requests.get(url, headers=headers)

    # Check if the request is OK
    if response.status_code == 200:
        response_json = response.json()
        model_link = response_json.get('source', {}).get('url')
        return model_link
    else:
        print(f"Errore durante il recupero del link di download: {response.status_code}")
        return None

def create_folder_if_not_exists(path):
    """
    Create the folder if it doesn't exist.
    """
    if not os.path.exists(path):
        os.makedirs(path)
    else:
        print(f"La cartella esiste già: {path}")

def unzip_file(zip_path, extract_to):
    """
    Unzips the specified ZIP file into the specified folder.
    """
    try:
        # Ensure the destination directory exists
        create_folder_if_not_exists(extract_to)

        # Extract the ZIP file
        with zipfile.ZipFile(zip_path, 'r') as zip_ref:
            zip_ref.extractall(extract_to)
    except zipfile.BadZipFile:
        print(f"Errore: il file {zip_path} non è un file ZIP valido.")
    except Exception as e:
        print(f"Errore durante l'estrazione del file ZIP: {e}")

print("Welcome to the Sketchfab Server")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print('Server in ascolto su', HOST, PORT)
    conn, addr = s.accept()
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(102400)
            if not data:
                break
            print(data.decode())  # Uid sent by the Unity Framework Download Button
            name, uid = data.decode().split(" ", 1)

            # Ensure the base save path exists
            create_folder_if_not_exists(base_save_path)

            # Downloading the 3D Model
            download_url = get_download_link(uid, api_token)
            if not download_url:
                print("Download URL non trovato. Riprova.")
                continue

            zip_path = os.path.join(base_save_path, f"{name}.zip")

            # File Saving
            response = requests.get(download_url, stream=True)
            with open(zip_path, 'wb') as f:
                for chunk in response.iter_content(chunk_size=1024):
                    if chunk:
                        f.write(chunk)


            # Unzipping the file
            extract_to = os.path.join(base_save_path, name)  # Extract to a folder with the same name as the model
            unzip_file(zip_path, extract_to)

            os.remove(zip_path)





