var JSWebSocket = {
    $webSocketState: {
        socket : null,

        onOpen:null,
        onClose:null,
        onError:null,
        onMessage:null
    },

    Connect: function(url){
        var newWebSocket = new WebSocket(UTF8ToString(url));

        webSocketState.socket = newWebSocket;

        newWebSocket.binaryType = 'arraybuffer';

        newWebSocket.onopen = webSocketState.onOpen;
        newWebSocket.onclose = webSocketState.onClose;
        newWebSocket.onerror = webSocketState.onError;
        newWebSocket.onmessage = webSocketState.onMessage;
    },

    SetOnOpenCallback: function(callback){
        webSocketState.onOpen = function() {
            Module['dynCall_v'](callback);
        }
    },

    SetOnCloseCallback: function(callback){
        webSocketState.onClose = function() {
            Module['dynCall_v'](callback);
        }
    },

    SetOnErrorCallback: function(callback){
        webSocketState.onError = function() {
            Module['dynCall_v'](callback);
        }
    },

    SetOnMessageCallback: function(callback){
        webSocketState.onMessage = function(event) {
            var rawdata = event.data;

            var bufferSize = lengthBytesUTF8(rawdata) + 1;
            //Allocate memory space
            var buffer = _malloc(bufferSize);
            //Copy old data to the new one then return it
            stringToUTF8(rawdata, buffer, bufferSize);

            Module['dynCall_vi'](callback, buffer);
        }
    },

    Send: function(message){
        var SendUTF8Message = UTF8ToString(message);
        webSocketState.socket.send(SendUTF8Message);
    },

    Close: function(code, reason){
        var UTF8Reason = UTF8ToString(reason);
        webSocketState.socket.close(code, UTF8Reason);
    },
}

autoAddDeps(JSWebSocket, '$webSocketState');
mergeInto(LibraryManager.library, JSWebSocket);