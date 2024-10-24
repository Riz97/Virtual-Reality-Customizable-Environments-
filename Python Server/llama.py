
import ollama
import socket
import time
HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 1234        # Port to listen on (non-privileged ports are > 1023)
model = "llama3.1"
prompt = "C# script code , with no comments "

print("Welcome to the LLAMA Python server for this Framework")

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
                time.sleep(5)
             
         
                conn.sendall(response.encode())    