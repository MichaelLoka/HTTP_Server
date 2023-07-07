# HTTP Server
### Network Application to connect a Client with an HTTP Server

### Implement part of the HTTP protocol.
- Threaded (multiple clients)
- GET only.
- Error handling
- Page Not Found
- Bad Request
- Redirection
- Internal Server Error

### Starting the Server
- Accept multiple clients by starting a thread for each accepted connection.
- Keep on accepting requests from the remote client until the client closes the socket (sends a zero-length message)
- For each received request, the server must reply with a response.

### Receiving Request
- The received request must be a valid HTTP request, else return 400 Bad Request
- Check the single space separating the request line parameters.
- Method URI HTTPVersion
- Check the blank line separating the header lines and the content, even if the content is empty
- Check valid URI
- Check at least the request line and host header and blank lines exist.

### Response Headers
- Content-Type: text/HTML
- Content-Length: The length of the content
- Date: Current DateTime of the server
- Location: Only if there is redirection.
