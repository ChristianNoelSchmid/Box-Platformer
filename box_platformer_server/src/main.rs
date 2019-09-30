use std::collections::HashMap;
use std::net::{TcpListener, TcpStream, Shutdown, SocketAddr};
use std::io::prelude::*;
use std::thread;
use std::sync::Arc;
use std::sync::Mutex;

use box_platformer_server::PlayerPackage;
use std::time::Duration;

fn main()
{
    let listener = TcpListener::bind("0.0.0.0:7878").unwrap();

    let host_msg : Arc<Mutex<HashMap<String, PlayerPackage>>> = Arc::new(Mutex::new(HashMap::new()));

    let host_msg_clear_clone = host_msg.clone();
    thread::spawn(move || {
        loop {
            let host_msg_unwrapped = host_msg_clear_clone.lock().unwrap();

            if(host_msg_unwrapped.values().len() > 0) 
            {
                for value in host_msg_unwrapped.values()
                {
                    print!("{}", value);
                    println!();
                }
                println!();
                std::io::stdout().flush().unwrap();
            }
            
            thread::sleep(Duration::from_millis(10));
        }
    });

    for stream in listener.incoming()
        {
            let clone_host_msg = host_msg.clone();
            thread::spawn(move || {
                match stream
                    {
                        Ok(s) => {
                            println!("New Connection from {}", s.local_addr().unwrap());
                            handle_player_connection(s, clone_host_msg);
                        },
                        Err(e) => { println!("{}", e) }
                    }
            });
        }
}

fn handle_player_connection(mut stream: TcpStream,
                            host_msg: Arc<Mutex<HashMap<String, PlayerPackage>>>)
{
    loop
        {
            //let sw = stopwatch::Stopwatch::start_new();
            let mut buffer = [0; 64];
            let read_result = stream.read(&mut buffer);

            match read_result
                {
                    Ok(s) => {

                        let data = String::from_utf8_lossy(&buffer[..]).trim_end_matches('\0').to_string();
                        let package = PlayerPackage::from_str(&data);

                        match package
                            {
                                Ok(p) => {
                                    host_msg.lock().unwrap().insert (
                                        p.get_key(),
                                        p
                                    );
                                },
                                Err(e) => println!("{}", e),
                            };

                        let msgs = host_msg.lock().unwrap();
                        for msg in msgs.keys()
                        {
                            stream.write(format!(">{}", msgs[msg].to_data_string()).as_bytes());
                        }
                        stream.flush().unwrap();
                    }
                    Err(e) => {
                        stream.shutdown(Shutdown::Both);
                        host_msg.lock().unwrap().clear();
                        println!("{}", e);
                        break;
                    }
                }
            //println!("{}", sw.elapsed_ms())
            thread::sleep(Duration::from_millis(10));
        }
}

