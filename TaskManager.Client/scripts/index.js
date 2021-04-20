// JavaScript source code
let taskContainer = document.querySelector(".tasks-container");

async function getTasks() {
    let tasks = await axios("https://localhost:5001/api/task");
    return tasks.data;
}

async function printTask() {
    taskContainer.innerHTML = "";
    let tasks = await getTasks();

    tasks.forEach(task => {
        taskContainer.innerHTML +=
        `
            <div class="task">
                <input type="checkbox" name="done" ${task.state == 2 ? "checked" : null}>
                <span>${task.name}</span>
            </div>
        `;
    });
    
}

// Get the modal
var modal = document.getElementById("myModal");

// Get the button that opens the modal
var btn = document.getElementById("newTask");

// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

let btnCancel = document.getElementById("btnCancel");
let btnCreate = document.getElementById("btnCreate");

btnCancel.addEventListener("click", () => {
    modal.style.display = "none";
});

// When the user clicks on the button, open the modal
btn.onclick = function () {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function () {
    modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

btnCreate.addEventListener("click", () => {
    CreateTask();
    modal.style.display = "none";
    printTask();
});

function CreateTask() {
    let taskName = document.getElementById("taskName").value;
    let today = new Date();

    let month = (today.getMonth() + 1) < 9 ? "0" + (today.getMonth() + 1) : (today.getMonth() + 1)
    let day = today.getDate() < 9 ? "0" + today.getDate() : today.getDate();
    let year = today.getFullYear();

    var date = year + '-' + month + '-' + day;

    let peticion = fetch("https://localhost:5001/api/task", {
            method: "POST",
            body: `{
                    "id": 0,
                    "createdOn": "${date}",
                    "name": "${taskName}",
                    "state": 0
                    }`,
            headers: {"Content-type": "application/json"}
        });

        peticion.then(res => res.json())
                .then(res => console.log(res))

}


printTask();
