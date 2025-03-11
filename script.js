
const button = document.getElementById("shield")
button.addEventListener('click', () => {
    /* Do we have a shorter way to write "document.querySelector(...)" for each feather? */
    const elements = document.querySelectorAll(
        ".wrapper, .shield-container, .feather, .feather2, .feather3, .clock-container"
    );

    elements.forEach(element => {
        element.style.animationPlayState = "running";
    });

    document.getElementById("wingSoundEffect").play();
    document.getElementById("song").play();
});



/* Implement a feature where pressing the 'H' key hides all HTML forms on the website, and 
pressing it again makes them reappear.. But when user presses 'h' when they are writing their name, it shouldnt hide it. */
document.addEventListener("keydown", function (event) {
    if (event.key.toLowerCase() === "h")
    {
        let activeElement = document.activeElement;
        if(activeElement.tagName.toLowerCase() === "input")
        {
            return;
        }

        let wrapper = document.querySelector(".wrapper");
        if (wrapper) {
            if (wrapper.style.visibility === "hidden") {
                wrapper.style.visibility = "visible";
                wrapper.style.opacity = "1";
            } else {
                wrapper.style.visibility = "hidden";
                wrapper.style.opacity = "0";
            }
        }
    }
});


const logins = [];
document.getElementById("submit").addEventListener("click", function(event) {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;

    logins.push({username, password});
    
    for(var i=0; i < logins.length; i++)
    {
        console.log(logins[i]);
    }

    event.preventDefault();
});

/* How to add a live clock that updates real time? */
function updateClock() {
    const now = new Date();
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');
    document.getElementById("clock").textContent = `${hours}:${minutes}:${seconds}`;
}

// Update clock every second
setInterval(updateClock, 1000);

// Initialize clock immediately
updateClock();