
/* Do we have a shorter way to write "document.querySelector(...)" for each feather? */
const button = document.getElementById("shield")
button.addEventListener('click', () => {
    const elements = document.querySelectorAll(
        ".wrapper, .shield-container, .feather, .feather2, .feather3"
    );

    elements.forEach(element => {
        element.style.animationPlayState = "running";
    });

    document.getElementById("wingSoundEffect").play();
    document.getElementById("song").play();
});

/* When I click on Login, it resets my website. I want it to be like a submit button not reset */
const loginForm = document.getElementById("loginForm");
loginForm.addEventListener("submit", function(event) {
    event.preventDefault();
});