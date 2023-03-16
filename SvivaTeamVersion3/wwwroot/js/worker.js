var intervalID = setInterval(myCallback, 500);

function myCallback() {
    var nav = getElementById("navigation-bar");

    if (screen.width >= 1200) {
        console.log(screen.width, 'Stay In desktop version');
    }
    else {
        console.log(screen.width, 'Switch to mobile version');
    }
}

//self.addEventListener('message', function (e) {
//    var message = e.data + 'checking!';
//    self.postMessage(message);
//})

