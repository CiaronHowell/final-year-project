const openCloseNavButton = document.getElementById('openCloseNavButton');
openCloseNavButton.addEventListener('click', (event) => {
    event.preventDefault()

    openCloseNavBar()
});

function openCloseNavBar () {
    const currentWidth = document.getElementById("nav").style.width

    // Alternate between open and closed
    document.getElementById("nav").style.width = currentWidth === "250px" ? "0px" : "250px"
}

// // Handles all clicks buttons within the project nav
// const notesNav = document.getElementById('nav');
// notesNav.addEventListener('click', async (event) => {
//     event.preventDefault()

//     console.log("created event listener for button clicks");

// });