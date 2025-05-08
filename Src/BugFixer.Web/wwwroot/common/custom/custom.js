function OpenAvatarInput() {
    $("#UserAvatar").click();
}

function UploadUserAvatar(url) {

    var avatarInput = document.getElementById("UserAvatar");

    if (avatarInput.files.length) {

        var file = avatarInput.files[0];

        var formData = new FormData();

        formData.append("userAvatar", file);

        $.ajax({
            url: url,
            type: "POST",
            data: formData,
            //When using FormData in an AJAX request, setting contentType: false 
            //ensures the browser automatically handles the Content- Type header correctly for multipart data.Setting processData: false
            //prevents jQuery from converting the data into a query string, preserving the structure of the FormData object for proper file upload.
            //These settings are essential for handling file uploads effectively.
            contentType: false,
            processData: false,
            beforeSend: function () {
                StartLoading('#UserInfoBox')
            },
            success: function (response) {
                EndLoading('#UserInfoBox') // Hide the loading animation
                if (response.status === "success") {
                    location.reload();
                }
                else {
                    swal({
                        title: "خطاء!",
                        text: "فرمت فایل ارسال شده معتبر نمی باشد",
                        icon: "error",
                        button: "باشه",
                    });
                }
            },
            error: function () {
                EndLoading('#UserInfoBox') // Hide the loading animation 
                swal({
                    title: "خطاء!",
                    text: "عملیات با خطا مواجه شد لطفا مجدد تلاش کنید",
                    icon: "error",
                    button: "باشه",
                });
            }
        })
    }


}

function StartLoading(selector = 'body') {

    $(selector).waitMe({

        effect: 'bounce',

        text: 'لطفا صبر کنید ...',

        bg: 'rgba(255, 255, 255, 0.7)',

        color: '#000'

    });
}

function EndLoading(selector = 'body') {
    $(selector).waitMe('hide');
    //$(selector).waitMe('hide', { effect: 'none' });
}

$("#CountryId").on("change", function () {
    var countryId = $("#CountryId").val();
    if (countryId !== '' && countryId.length) {
        $.ajax({
            url: $("#CountryId").attr("data-url"),
            type: "get",
            data: {
                countryId: countryId
            },
            beforeSend: function () {
                StartLoading();
            },
            success: function (response) {
                EndLoading();
                $("#CityId option:not(:first)").remove();
                $("#CityId").prop("disabled", false);
                for (var city of response) {
                    $("#CityId").append(`<option value="${city.id}">${city.title}</option>`);
                }
            },
            error: function () {
                EndLoading();
                swal({
                    title: "خطا",
                    text: "عملیات با خطا مواجه شد لطفا مجدد تلاش کنید .",
                    icon: "error",
                    button: "باشه"
                });
            }
        });
    }
    else {
        $("#CityId option:not(:first)").remove();
        $("#CityId").prop("disabled", true);
    }
});

var datepickers = document.querySelectorAll('.datepicker');

if (datepickers.length) {
    for (datepicker of datepickers) {
        var id = $(datepicker).attr('id');
        kamaDatepicker(id, {
            placeholder: "مثال : 1400/01/01",
            twodigit: true,
            closeAfterSelect: true,
            forceFarsiDigits: true,
            markToday: true,
            markHolidays: true,
            highlightSelectedDay: true,
            sync: true,
            gotoToday: true
        });
    }
}

$(function () {
    if ($("#CountryId").val() === "") {
        $("#CityId").prop("disabled", true);
    }
});

var editors = document.querySelectorAll(".editor");
if (editors.length) {
    $.getScript("/common/ckeditor/build/ckeditor.js",
        function (data, textStatus, jqxhr) {
            for (editor of editors) {
                ClassicEditor
                    .create(editor,
                        {
                            licenseKey: '',
                            simpleUpload: {
                                uploadUrl: '/Home/UploadEditorImage'
                            }
                        })
                    .then(editor => {
                        window.editor = editor;
                    })
                    .catch(error => {
                        console.log(error);
                    });
            }
        });
}

function SubmitQuestionForm() {

    $("#filter_form").submit();
}

function SubmitFilterFormPagination(pageId) {
    $('#CurrentPage').val(pageId);
    $("#filter_form").submit();
}
