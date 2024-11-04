
import socket
import time
from openai import OpenAI
HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 1234        # Port to listen on (non-privileged ports are > 1023)
model = "gpt-4o-mini"
client = OpenAI(api_key = "") #Insert the OpenAI API key

print("Welcome to the Codex Python server for this Framework")

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
                completion = client.chat.completions.create(
                model="gpt-4o-mini",
                messages=[
                {"role": "system", "content": "You are a helpful assistant."},
                {
                "role": "user",
                "content": data.decode()
                }
             ]
            )
                response = completion.choices[0].message.content
                print(response)
                     
                conn.sendall(response.encode())   
                time.sleep(5)  



