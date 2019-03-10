function Paging_load(path) {
    Paging_ShowSpinner(true);
    $("#Layout_frame_content").load(path, function () {
        Paging_ShowSpinner(false);
    });
}
function Paging_ShowSpinner(show) {
    if (show) {
        $("#Layout_spinner_layer").show();
        $("#Layout_frame").stop(true, true).fadeOut('slow');
        $("#Layout_frame_content").prop('disabled', true);
    }
    else {
        $("#Layout_spinner_layer").hide();
        $("#Layout_frame").stop(true, true).fadeIn('slow');
        $("#Layout_frame_content").prop('disabled', false);
    }
}

function Paging_uploadeditingpageswitch() {
    $("#gotomyupload").click(function (event) {
        event.preventDefault();
        Paging_load("./myupload.html");
    });
    $("#gotouploadhistory").click(function (event) {
        event.preventDefault();
        Paging_load("./uploadhistory.html");
    });
}
$(document).ready(function () {
    Paging_uploadeditingpageswitch();
});