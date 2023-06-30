// Get the date input element
const expireDateInput = document.getElementById('expireDate');

// Get today's date
const today = new Date();

// Calculate the date 30 days from today
const maxDate = new Date();
maxDate.setDate(today.getDate() + 30);

// Format the max date as yyyy-MM-dd
const maxDateString = maxDate.toISOString().split('T')[0];

// Set the min and max attributes of the date input
expireDateInput.min = today.toISOString().split('T')[0];
expireDateInput.max = maxDateString;

document.getElementById("hiddenTitle").value = "Accident";
document.getElementById("Road_Problems1").addEventListener("change", function () {
    document.getElementById("hiddenTitle").value = document.getElementById("Road_Problems1").value;
})

/*Stages Script*/
var first_stage = document.getElementById("stg1");
var second_stage = document.getElementById("stg2");
var third_stage = document.getElementById("stg3");

var prev_report = document.getElementById("Prev-report");
var next_report = document.getElementById("Next-report");

var stage = 1;
var page = 1;

const progressBar = document.getElementById("progress-bar");
let progress = 34;
updateProgressBar();

next_report.addEventListener("click", function () {
    if (progress > 0) {
        page++;
        checkpage()
        if (page <= 3) {
            progress += 33;
            updateProgressBar();
        }
    }
});

prev_report.addEventListener("click", function () {
    if (progress <= 100) {
        page--;
        checkpage()
        if (page >= 1) {
            progress -= 33;
            updateProgressBar();
        }
    }
})

function checkpage() {
    if (page == 3) {
        //Disable the button & display the submit button.
        next_report.disabled = true;
        first_stage.style = "display:none;";
        second_stage.style = "display:none;";
        third_stage.style = "display:inline;";
    }
    else if (page == 2) {
        prev_report.disabled = false;
        next_report.disabled = false;
        first_stage.style = "display:none;";
        second_stage.style = "display:inline;";
        third_stage.style = "display:none;";
    }
    else if (page == 1) {
        //Disable prev button.
        prev_report.disabled = true;
        first_stage.style = "display:inline;";
        second_stage.style = "display:none;";
        third_stage.style = "display:none;";
    }
}

function updateProgressBar() {
    if (stage >= 1 && stage <= 3) {
        progressBar.style.width = progress + "%";
        if (progress >= 99) {
            progressBar.classList.add("green");
            progressBar.classList.remove("yellow");
        } else if (progress >= 66) {
            progressBar.classList.add("yellow");
            progressBar.classList.remove("green");
        } else {
            progressBar.classList.remove("yellow");
            progressBar.classList.remove("green");
        }
    }
}

var lat = 0;
var long = 0;

var layers = [new ol.layer.Tile({ source: new ol.source.OSM() })];

var map = new ol.Map({
    target: 'map',
    view: new ol.View({
        zoom: 5,
        center: [166326, 5992663]
    }),
    interactions: ol.interaction.defaults({ altShiftDragRotate: false, pinchRotate: false }),
    layers: layers
});

window.addEventListener('load', function () {
    checkpage();
});

var search = new ol.control.SearchPhoton({
    lang: "fr",
    reverse: true,
    position: true
});
map.addControl(search);

var markers;
search.on('select', function (e) {

    map.getView().animate({
        center: e.coordinate,
        zoom: Math.max(map.getView().getZoom(), 16)
    });
    loc(e.coordinate);
});



function loc(e) {
    lat = e[0];
    long = e[1];
    updateValue(lat, long);
    //console.log(lat, long); //MARKED
    if (markers) {
        map.removeLayer(markers)
    }

    markers = new ol.layer.Vector({
        source: new ol.source.Vector(),
        style: new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1],
                src: 'https://ucarecdn.com/4b516de9-d43d-4b75-9f0f-ab0916bd85eb/marker.png' // => https://app.uploadcare.com/projects/c05bbeb5e1a61e862903/files/7110cd57-b0ee-4833-bcd1-ff0343dd01cc/?limit=100&ordering=-datetime_uploaded
            })
        })
    });

    map.addLayer(markers);

    var marker = new ol.Feature(new ol.geom.Point(e));
    markers.getSource().addFeature(marker);
}

document.querySelector("#location-button").addEventListener("click", function (e) {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            console.log(position["coords"]["latitude"], position["coords"]["longitude"]); //MARKED
            if (markers) {
                map.removeLayer(markers)
            }

            lat = position["coords"]["latitude"];
            long = position["coords"]["longitude"];
            markers = new ol.layer.Vector({
                source: new ol.source.Vector(),
                style: new ol.style.Style({
                    image: new ol.style.Icon({
                        anchor: [0.5, 1],
                        src: 'https://ucarecdn.com/4b516de9-d43d-4b75-9f0f-ab0916bd85eb/marker.png' // => https://app.uploadcare.com/projects/c05bbeb5e1a61e862903/files/7110cd57-b0ee-4833-bcd1-ff0343dd01cc/?limit=100&ordering=-datetime_uploaded
                    })
                })
            });

            map.addLayer(markers);

            var marker = new ol.Feature(new ol.geom.Point(ol.proj.fromLonLat([position.coords.latitude, position.coords.longitude])));
            markers.getSource().addFeature(marker);
        });
    }
    else {
        alert("Your browser does not support geolocation");
    }
});

function updateValue(lat, long) {
    var submit_form = document.getElementById('AddReportToDB');

    document.getElementById("lat").value = lat;
    document.getElementById("long").value = long;


    if (submit_form.disabled == true) {
        submit_form.disabled = false;
        document.getElementById("SubmitStatus").title = "Submit";
    }
}