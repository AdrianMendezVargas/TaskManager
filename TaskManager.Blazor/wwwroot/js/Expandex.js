

function addExpandex() {
    var coll = document.getElementsByClassName("collapsible");
    var i;

    for (i = 0; i < coll.length; i++) {
        let content = coll[i].nextElementSibling;
        if (coll[i].classList.contains("collapsible-active")) {
            content.style.maxHeight = content.scrollHeight + "px";
        } else {
            content.style.maxHeight = null;
        }
        addClickListener(coll[i]);
    }
}

function addClickListener(element) {
    element.firstElementChild.addEventListener("click", function () {
        this.parentElement.classList.toggle("collapsible-active");
        let content = this.parentElement.nextElementSibling;
        if (content.style.maxHeight) {
            content.style.maxHeight = null;
        } else {
            content.style.maxHeight = content.scrollHeight + "px";
        }
    });
}