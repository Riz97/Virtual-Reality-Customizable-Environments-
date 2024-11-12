import ollama
import socket
import time
HOST = '127.0.1.5'  # Standard loopback interface address (localhost)
PORT = 12347        # Port to listen on (non-privileged ports are > 1023)
model = "codellama"


print("Welcome to the CODELLAMA Python server for this Framework")

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST,PORT))
    s.listen()
    conn, addr = s.accept()
    print('Server in ascolto su', HOST, PORT)
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(102400)
            print(data.decode())      
            if(data.decode()!= "STOP"):
                ollama_response = ollama.chat(
                model = model,
                messages = [{'role': 'user', 'content': data.decode()}],
                
                )
                response = ollama_response['message']['content']
                print(response)
                     
                conn.sendall(response.encode())   
                time.sleep(5)  