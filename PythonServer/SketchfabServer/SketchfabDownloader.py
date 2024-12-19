import requests
import os
import socket
import zipfile
import shutil


api_token = "vvAlrUNsMbiGG02w7So1CDE61eimIY" #Sketchfab API Token
HOST = '127.0.1.10'
PORT = 12321 

#Path where it is going to be saved the unzipped file
base_save_path = "C:/Users/ricky/Desktop/Framework/Virtual-Reality-Customizable-Environments-/Assets/Imported/"

#Path where we are going to save just the .fbx files
fbx_save_path = "C:/Users/ricky/Desktop/Framework/Virtual-Reality-Customizable-Environments-/Assets/Resources/ImportedFBX/"

#Get the 3d object uid 
def get_download_link(model_uid, api_token):
    url = f"https://api.sketchfab.com/v3/models/{model_uid}/download"
    headers = {"Authorization": f"Bearer {api_token}"}

    response = requests.get(url, headers=headers)

    if response.status_code == 200:
        response_json = response.json()
        model_link = response_json.get('source', {}).get('url')
        return model_link
    else:
        print(f"Error in retrieving the download link:  {response.status_code}")
        return None


#Create the folder if it does not exist
def create_folder_if_not_exists(path):
 
    if not os.path.exists(path):
        os.makedirs(path)

#Extract the zip file in the desired folder
def unzip_file(zip_path, extract_to):

    try:
        create_folder_if_not_exists(extract_to) #Create the folder if it does not exist

        with zipfile.ZipFile(zip_path, 'r') as zip_ref:
            zip_ref.extractall(extract_to) #Extract the file
    except zipfile.BadZipFile:
        print(f"Error: the zip file {zip_path} is not valid.")
    except Exception as e:
        print(f"Error: extraction failed: {e}")

#Search recursively the .fbx files in the just unzipped folders
def find_fbx_files(directory_path):
    fbx_files = []
    for root, dirs, files in os.walk(directory_path):  # os.walk is recursive
        for file in files:
            if file.endswith('.fbx'):
                fbx_files.append(os.path.join(root, file))  # Add the complete .fbx path
    return fbx_files

#Remove the space in the .fbx's name, otherwise we would had trouble in the Unity Natural Language Request
def sanitize_file_name(file_name):

    return file_name.replace(" ", "_")

#Copy the .fbx files in the destination folder and remove the spaces in the models' names
def copy_fbx_files(fbx_files, destination_directory):
 
    create_folder_if_not_exists(destination_directory)

    for fbx_file in fbx_files:
        sanitized_name = sanitize_file_name(os.path.basename(fbx_file))  
        destination_fbx = os.path.join(destination_directory, sanitized_name)

        try:
            shutil.copy(fbx_file, destination_fbx) #Copy it
            #If everything is ok
            print(f"File {fbx_file} copied as{sanitized_name} in {destination_directory}.")
        except Exception as e:
            print(f"Error in copying {fbx_file}: {e}")

#Search recursively .zip files in the subfolders, extract them and search for .fbx files.
#If there aren't any fbx files delete the directory, and so the 3D object cannot be imported in Unity
def process_nested_zips(directory_path):

    for root, dirs, files in os.walk(directory_path): 
        for file in files:
            if file.endswith('.zip'):
                zip_path = os.path.join(root, file)
                temp_extract_path = os.path.splitext(zip_path)[0]  

                # Extract the zip file
                unzip_file(zip_path, temp_extract_path)

                # Search for an .fbx file
                fbx_files = find_fbx_files(temp_extract_path)

                if fbx_files:
                    #if there is one copy it
                    copy_fbx_files(fbx_files, fbx_save_path)
                else:
                    #Otherwise delete the directory
                    delete_folder_and_contents(temp_extract_path)

                #And delete the original zip
                os.remove(zip_path)

#Delete the folder and what it contains
def delete_folder_and_contents(folder_path):

    try:
        if os.path.exists(folder_path):
            shutil.rmtree(folder_path)
            print(f"Deleted folder: {folder_path}")
        else:
            print(f"Error: The folder {folder_path} does not exist.")
    except Exception as e:
        print(f"Error in the folder deletion process {folder_path}: {e}")



print("Welcome to the Sketchfab Server")

#Server Activation
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

            #Chck if the folder exists
            create_folder_if_not_exists(base_save_path)

            # Retrieve the model download link
            download_url = get_download_link(uid, api_token)
            if not download_url:
                print("URL not found. Retry")
                continue

            zip_path = os.path.join(base_save_path, f"{name}.zip")

            # Download the zip file
            response = requests.get(download_url, stream=True)
            with open(zip_path, 'wb') as f:
                for chunk in response.iter_content(chunk_size=1024):
                    if chunk:
                        f.write(chunk)

            # Extract it
            extract_to = os.path.join(base_save_path, name)
            unzip_file(zip_path, extract_to)
            os.remove(zip_path)
        
            # Handle nested zips in the original zip file
            process_nested_zips(extract_to)

            # Search for .fbx in the original folder
            fbx_files = find_fbx_files(extract_to)
            if fbx_files:
                copy_fbx_files(fbx_files, fbx_save_path)
                print(fbx_files)
                #Send to the client the string ok that represents a succesful download
                content = "OK"
                conn.sendall(content.encode())
                
            else:
                #Otherwise there is not a single .fbx and so to the client is sent a NOT OK string
                delete_folder_and_contents(extract_to)
                content = "NOT OK"
                conn.sendall(content.encode())
                
             