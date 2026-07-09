//Refenced and modified offical Godot Signaling-RTC demo at https://github.com/godotengine/godot-demo-projects under MIT license

const http = require("http"); // adding http service for deployment
const WebSocket = require("ws"); // Using WebSocket package
const PORT = process.env.PORT || 8080; //use default port or service provided port

const httpServer = http.createServer();
const wss = new WebSocket.Server({ server: httpServer });
httpServer.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});

const rooms = new Map();

wss.on("connection", (ws) => {
	console.log("Client connected");
  let roomName = null; //init roomName at start of connection
  // When a message is received from this client
  ws.on("message", (msg) => {
    let data;
	console.log("message received");
    // Try to parse incoming message as JSON
    try {
      data = JSON.parse(msg);
    } catch {
      // Ignore invalid JSON messages
      console.log("Invalid JSON received");
      return;
    }

    // Extract room name from message
    const room = data.room;
    if (!room) return; // Ignore if no room specified

	// Create room if it doesn't exist
	if (!rooms.has(room)) {
		rooms.set(room, {
			clients: new Set(),
			matchReadySent: false
		});
    }
	const roomRef = rooms.get(room);
    // Let client join the room
    if (!roomName) {
		roomName = room; 
		const clients = roomRef.clients; // Get all clients in that room
		roomRef.clients.add(ws); // Add this client to the room
		const playerCount = roomRef.clients.size;
		console.log("Client joined room:", room);
		console.log("Room size: ", playerCount);
		for(const client of clients){ // Send player count update to all players
			if (client.readyState === WebSocket.OPEN){
				client.send(JSON.stringify({
					room: room,
					type: "playerCountUpdate",
					count: playerCount
				}));
			}
		}
		if (playerCount === 2 && !roomRef.matchReadySent){ // Send ready message when both player joined
			roomRef.matchReadySent = true;
			for (const client of roomRef.clients) {
				if (client.readyState === WebSocket.OPEN) {
					client.send(JSON.stringify({
					room: room,
					type: "matchReady"
					}));
				}
			}
		console.log(room + ": is ready and full");
		}
	}
	for(const client of roomRef.clients) { // Core relay function (excludes sender)
		if (client !== ws && client.readyState === WebSocket.OPEN) {
			client.send(JSON.stringify(data));
		}
	}
  });

  // When the client disconnects
  ws.on("close", () => {
    if (roomName && rooms.has(roomName)) {
      // Remove client from the room
	  const roomRef = rooms.get(roomName);
      roomRef.clients.delete(ws);
      // If room is empty, delete it
      if (roomRef.clients.size === 0) {
        rooms.delete(roomName);
		console.log(`Room: ${roomName} is now empty and deleted`);
      }
      console.log("Client left room:", roomName);
	  roomRef.matchReadySent = false;
    }
  });
});

// Startup message
console.log("Signaling server running on Port: " + PORT);
