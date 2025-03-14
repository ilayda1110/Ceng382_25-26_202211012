document.getElementById("classForm").addEventListener("submit", function(event)
{
    event.preventDefault(); // Prevent form from reloading the page

    // Get form values
    const className = document.getElementById("className").value.trim();
    const numPeople = document.getElementById("numPeople").value.trim();
    const description = document.getElementById("description").value.trim();

    if (!className || !numPeople || !description)
    {
        alert("Please fill in all fields!");
        return;
    }

    // Create a new row
    const tableBody = document.getElementById("classTableBody");
    const newRow = document.createElement("tr");

    newRow.innerHTML = `
        <td>${className}</td>
        <td>${numPeople}</td>
        <td>${description}</td>
        <td><button class="delete-btn">Delete</button></td>
    `;
    
    // Append the row to the table
    tableBody.appendChild(newRow);
    

/*
    Customize my delete button like my "Add class" button
*/
    // ✅ Style Delete Button
    const deleteButton = newRow.querySelector(".delete-btn");
    deleteButton.classList.add("styled-button"); // Apply the shared button style    



/*  
    Also add this:
    When a row is double-clicked, additional actions like displaying detailed 
    information or removing the row are triggered
*/

    // Add event listener to delete button
    newRow.querySelector(".delete-btn").addEventListener("click", function()
    {
        newRow.remove(); // Remove the row when delete button is clicked
    });

    // ✅ Add double-click event to show detailed info
    newRow.addEventListener("dblclick", function()
    {
        alert(`Class: ${className}\nNumber of People: ${numPeople}\nDescription: ${description}`);
    });


    // ✅ Add hover effect
    newRow.addEventListener("mouseenter", function()
    {
        newRow.style.backgroundColor = "lightgray";
    });

     newRow.addEventListener("mouseleave", function()
    {
        newRow.style.backgroundColor = "";
    });

    // Clear the form fields
    document.getElementById("classForm").reset();
});