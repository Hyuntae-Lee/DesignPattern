## CommMngr
1. It is a independant module. (a project in a solution)
2. It has following interfaces
   * OpenConnection(ip: string, port: int) : bool
      - Open TCP connection with ip and port as a client.
   * CloseConnection() : void
      - Close opened connection.
   * SendCommand(msg: string, bWaitForResp: bool) : void
      - Send text message to the connected server.
   * IsConnected() : bool
   * OnRespReceive(msg: string) : void
      - Called when a response received for the sent command.
   * OnCommentReceived(msg: string) : void
      - Called when any not formatted text is received.
3. It recognize command by following format
   * [ABC_DDD_VVVV...]
   * The command should be start with '[' and end with ']'.
   * The first 3 characters of 'ABC' are head part.
      - The first character 'A' in the format means request or response.
      - It should be 'S' if it is request.
      - It should be 'E' if it is response.
      - The other two characters express connection owners.
   * The second 3 characters of 'DDD' in the format express command.
      - It has an unique meaning.
   * The last characters are value for the command.
      - They don't have length limit.
   * Any texts cannot satisfy this format are comments.
4. The request and response match by first two phrases.
   * To match the request and the response, they have same 'BC' and 'DDD'.
   * The request's 'A' should be 'S' and the response's 'A' should be 'E'.
5. It can send commands sequencely.
   * After send a command it should wait for a response for the sent command.
   * All requested commands by client during the waiting period should be queued.
   * The queued commands should be sent sequencely after the previous command reponsed.
6. The client can determine wait for response or not by bWaitForResp parameter in SendCommand().
   * If bWaitForResp is false, CommMngr should not wait for the response for this command.
