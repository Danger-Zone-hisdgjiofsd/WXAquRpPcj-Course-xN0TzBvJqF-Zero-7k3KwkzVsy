function get_thumbnail(thumbnail_id) {
    var msg_to_send_toGetThumbnail = {
        "auth_token": "gefq4xhs7catajt6j8zpjp3m3dmndhu+fgwsu48rv3j1n20nwuvgl/ysjgtyfc3iqgs0sn8onow9idbrtu1jd+vg7ofbm0lrfoxyfrenklkwfpavsbeatbuy7/jh7c8s",
        "file_ID": thumbnail_id
    }
    postJSON("/api/File/GetThumbnailByFileid", (msg_to_send_toGetThumbnail), function (obj) {
        $("#zero").append("<div><img src ='./material/blank-profile-picture.png'></div>");
        Search_result_page_thumbnail_path = window.URL.createObjectURL(obj);    
    });
}

function aaaaa() {
    var Search_result_page_thumbnail_path = "./material/preview.png"
    var Search_result_page_profile_path = "./material/blank-profile-picture.png"
    var Search_result_details = "empty"
    var msg_to_send = {
        "auth_token":"gefq4xhs7catajt6j8zpjp3m3dmndhu+fgwsu48rv3j1n20nwuvgl/ysjgtyfc3iqgs0sn8onow9idbrtu1jd+vg7ofbm0lrfoxyfrenklkwfpavsbeatbuy7/jh7c8s",//g_auth_token,
        "search_query": "engineering",
        "next_20": 0
    }
    var file_spec = {
        "file_ID": -1,
        "file_Name": "null",
        "file_Typename": "null",
        "file_Description": "null",
        "related_courseID": -1,
        "uploader_UserID": -1,
        "uploader_Username": "null",
        "upload_Time": "null",
        "likes": -1,
        "disLikes": -1
    }

    postJSON("/api/Search/SearchByQuery", (msg_to_send), function (obj) {
        if (obj["status_code"] == 0) {
            for (var i = 0; i < obj["result"].length; i++) {

                Search_result_details = "file name : " + obj["result"][i]["file_Name"] + "<br>";
                Search_result_details += "file type : " + obj["result"][i]["file_Typename"] + "<br>";
                var searchTNtemp = "Search_result_thumbnail" + i;
                

                $("#one").append('<div class="container-fluid that_result"><div class= "row border-bottom border-dark" ><div class="col-3 align-self-center file_thumbnail"><img id = "Search_result_thumbnail' + i + '"></div><div class="col-6 align-self-center">' + '<div class="row" id="Search_result_filename"><h3>' + obj["result"][i]["file_Name"] + '</h3></div><div class="row" id="Search_result_description">File Description:<br>' + obj["result"][i]["file_Description"] + '</div><div class="row"><div class="col-4" id="Search_result_type">' + obj["result"][i]["file_Typename"] + '</div><div class="col-4" id="Search_result_like">like: ' + obj["result"][i]["likes"] + '</div><div class="col-4" id="Search_result_unlike">unlike: ' + obj["result"][i]["disLikes"] + '</div></div>' + '</div><div class="col-3 avatar"><img class="rounded-circle" src=' + Search_result_page_profile_path + '></div></div></div>');
                SetResource_URL("../api/File/GetThumbnailByFileid", "file_id", obj["result"][i]["file_ID"], searchTNtemp, "src");
            }
        }
    });
}


function SetResource_URL(url, resourcekey, resourceid, elementid, attribute) {
    var data = {
        "auth_token": "gefq4xhs7catajt6j8zpjp3m3dmndhu+fgwsu48rv3j1n20nwuvgl/ysjgtyfc3iqgs0sn8onow9idbrtu1jd+vg7ofbm0lrfoxyfrenklkwfpavsbeatbuy7/jh7c8s",
    }
    data[resourcekey] = resourceid;
    return jQuery.ajax({
        'type': 'POST',
        'cache': true,
        'url': url,
        'contentType': 'application/json',
        'xhr': function () {
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob'
            return xhr;
        },
        'data': JSON.stringify(data),
        'success': function (object) {
            $("#" + elementid).attr(attribute, window.URL.createObjectURL(object));
        }
    })
};