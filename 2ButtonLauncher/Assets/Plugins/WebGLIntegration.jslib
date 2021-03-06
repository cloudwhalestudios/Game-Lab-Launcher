mergeInto(LibraryManager.library, {

    Redirect: function(str_location) {
        window.location.href = encodeURI(Pointer_stringify(str_location));
    },

    RedirectWithParams: function(str_location, str_paramsJson) {
        /* // Testing
        var str_location = 'http://www.cloudwhale.nl/launcher/games/jump-and-shoot/';
        var str_paramsJson = '{"completedSetup":true,"keys":[97,100],"menuProgressionTimer":2.0}';

        var jsonObject = JSON.parse(str_paramsJson);
        var baseUrl = encodeURI(str_location + '?');
        */
        var jsonObject = JSON.parse(Pointer_stringify(str_paramsJson));
        var baseUrl = encodeURI(Pointer_stringify(str_location) + '?');

        console.log("Received target url: " + baseUrl);
        console.log("Received json: " + JSON.stringify(jsonObject));

        var paramUrl = encodeURI(JSON.stringify(jsonObject));
        var url = encodeURI(baseUrl + paramUrl);

        console.log("Redirecting to: " + url);

        window.location.href = url;
    },

    Refresh: function() {
        location.reload();
    },

    GetParams: function () {
        var encodedUrl = decodeURI(window.location.toString());
        var decodedSearchUrl = decodeURI(decodeURI(encodedUrl).split('?')[1]);

        console.log("Decoded search url = " + decodedSearchUrl);

        var jsonObject = JSON.parse(decodedSearchUrl);
        var jsonString = JSON.stringify(jsonObject);

        console.log("Read parameters as json: " + jsonString);

        var bufferSize = lengthBytesUTF8(jsonString) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(jsonString, buffer, bufferSize);

        console.log("Parameter buffer: " + buffer);

        return buffer;
    },

    SetParams: function(str_paramsJson) {
        var jsonObject = JSON.parse(Pointer_stringify(str_paramsJson));
        var searchString = window.location.search = "";
        var questionmark = false;
        for(var key in jsonObject) {
            if (jsonObject.hasOwnProperty(key)) {
                // Attach to url
                if (!questionmark) {
                    searchString += "?";
                }
                else {
                    searchString += "&";
                }
                searchString += key + "=" + jsonObject[key];
            }
        }
        window.location.search = searchString;
    },

    SetTitle: function(str_title) {
        var exitCondition = Pointer_stringify(str_title);

        // Set the title to the exit condition for the java program to react upon
        document.title = exitCondition;
    },

    Crash: function() {
        window.alert("This should totally be crashing right now.");
    },
});