﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var mySelect = document.getElementById("lang-switch");
mySelect.onchange = (event) => {
    var choosen_language = event.target.value;

    var About_Title = document.getElementById("AboutProjectTitleText");
    var About_Body = document.getElementById("AboutProjectBody");
    var Report_Map_Title = document.getElementById("ReportMapTitleText");
    var Report_Map_Body = document.getElementById("ReportMapBody");
    var Subtitle = document.getElementById("subtitle");
    var Add_Report_Body = document.getElementById("AddReportBody");
    var Add_Report_Title = document.getElementById("AddReportTitleText");

    switch (choosen_language.toString()) {
        case "EN":
            About_Body.innerHTML = "This project is an entrepreneurship environmental, social, long-lasting project. Its purpose is to make accessible and efficient the organization of cleaning groups. If until today we organized beach cleaning groups through Facebook, and it was sometimes difficult to find an available group or in an area convenient for everyone. This interface offers a website that will centralize all the organizations in a notification map and thus also establish a faster contact with the nature authorities.";
            About_Body.style = "text-align: left"
            About_Title.innerHTML = "What is SvivaTeam?";
            document.getElementById("AboutProjectTitleTextAlign").style = "text-align: left"

            Report_Map_Body.innerHTML = "To optomize and help with the organization of cleaning and volunteer groups, we use a map where all registered users can report any ecological or social problem that he identified so other users can help solve the problem or report about the problem to the appropriate authorities.By pressing the button below you can proceed to the map to find reports you want to help solve. To learn more about a certain report press on the marker.";
            Report_Map_Body.style = "text-align: left"
            Report_Map_Title.innerHTML = "REPORT MAP";
            document.getElementById("ReportMapTitleTextAlign").style = "text-align: left"

            Subtitle.innerHTML = "BETTER WORLD IS MADE TOGETHER";

            Add_Report_Body.innerHTML = "Identified a dirty beach? Or an abandoned car? Add a report to our report map and find hardworking people who are willing to help you solve the problem.To leave a report you must be a registered user on our site, after registrasion you will ba able to report any social or ecological problem everywhere and everywhen.If you have a group on social media about this problem don`t forget to add a link in the problem description.";
            Add_Report_Body.style = "text-align: left"
            Add_Report_Title.innerHTML = "NEW REPORT";
            document.getElementById("AddReportTitleTextAlign").style = "text-align: left"
            
            break;
        case "HE":
            About_Body.innerHTML = "פרויקט זה הינו פרויקט יזמות סביבתי, חברתי, קיימותי. מטרתו היא להנגיש ולייעל את הארגון של קבוצות נקיון בארץ. אם עד היום ארגנו קבוצות נקיון חופים דרך הפייסבוק, והיה קשה לפעמים למצוא קבוצה פנויה או באזור נוח לכל אחד. הממשק הזה מציע אתר שירכז את כל ההתארגנויות במפת התראות וככה גם ייצור קשר מהיר יותר לרשויות הטבע.";
            About_Body.style = "text-align: right"
            About_Title.innerHTML = "?SvivaTeam מה זה";
            document.getElementById("AboutProjectTitleTextAlign").style = "text-align: right"

            Report_Map_Body.innerHTML = ".בכדי לייעל ולעזור בהתארגנות של קבוצות ניקיון ועזרה לזולת, אנו משתמשים במפה בה כל משתמש רשום יכול לדווח על בעיה סביבתית או חברתית אותה זיהה בכדי שמשתמשים אחרים יוכלו לעזור לפתור את הבעיה או לדווח עליה לרשויות המתאימות. בלחיצה על הכפתור למטה הנך יכול\ה לעבור לצפייה במפה וחיפוש של דיווחים שתרצה\י להשתתף בפתירתם. בכדי לקרוא עוד על דיווח מסוים לחץ על המרקר המתאים";
            Report_Map_Body.style = "text-align: right"
            Report_Map_Title.innerHTML = "מפת ההתראות";
            document.getElementById("ReportMapTitleTextAlign").style = "text-align: right"

            Subtitle.innerHTML = "יוצרים עולם טוב יותר בעזרת עבודת צוות";

            Add_Report_Body.innerHTML = "זיהית חוף מלוכלך? או אולי מכונית נטושה באמצע הכביש? הוסף דיווח חדש למפת ההתראות ותמצא אנשים חרוצים. שיעזרו לך לפתור את הבעיה! בשביל לדווח על בעיה כלשהי הנך צריך\ה להיות משתמש רשום באתר, לאחר ההרשמה תוכלו לדווח על בעיות סביבתיות או חברתיות בכל מקום, בכל רגע. אם יש לכם קבוצה ברשת חברתית כלשהי הנוגעת בבעיה אל תשכחו להוסיף את הקישור בתיאור הבעיה.";
            Add_Report_Body.style = "text-align: right"
            Add_Report_Title.innerHTML = "יצירת דיווח חדש";
            document.getElementById("AddReportTitleTextAlign").style = "text-align: right"
            break;
    }
}       

document.querySelector("#File_Upload").addEventListener("change", (e) => {
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        const files = e.target.files;
        const output = document.querySelector("#result");
        output.innerHTML = "";
        for (let i = 0; i < files.length; i++) {
            if (!files[i].type.match("image")) continue;
            const picReader = new FileReader();
            picReader.addEventListener("load", function (event) {
                const picFile = event.target;
                console.log(picFile.width)
                const div = document.createElement("div");
                div.innerHTML = `<img class="form-control" id="thumbnail" src="${picFile.result}" title="${picFile.name}" style="height: 500%;width: 200%;"/>`;
                output.appendChild(div);
            });
            picReader.readAsDataURL(files[i]);
            document.getElementById('display_image').setAttribute("style", "display:inline;width:auto");

        }
    } else {
        alert("Your browser does not support File API");
    }
});

let category = document.getElementById("Category");
let RP = document.getElementById("Road_Problems1");
let UP = document.getElementById("Urban_problems1");
let Other = document.getElementById("Other1");

category.addEventListener("click", function () {
    let cat = document.getElementById("Category").value;
    show(cat);
});

function show(category) {
    if (category == "Road_Problems") {
        console.log('1');
        RP.style = " display: inline"
        UP.style = "display: none"
        Other.style = " display: none"
    } if (category == "Urban_Problems") {
        console.log('2');
        RP.style = " display: none"
        UP.style = "display: inline"
        Other.style = " display: none"
    } if (category == "Other") {
        console.log('3');
        RP.style = " display: none"
        UP.style = "display: none"
        Other.style = " display: inline"
    }
}


function confirmDelete(uniqeId, isDeleteClicked) {

    let deleteSpan = document.getElementById('deleteSpan_' + uniqeId);
    let confirmDeleteSpan = document.getElementById('confirmDeleteSpan_' + uniqeId);

    if (isDeleteClicked) {
        deleteSpan.style = "display:none"
        confirmDeleteSpan.style = "display:inline"
    } else {
        deleteSpan.style = "display:inline"
        confirmDeleteSpan.style = "display:none"
    }
}

function togglePopup(id) {
    document.getElementById("overlay-container-" + id).classList.toggle("active");
}
