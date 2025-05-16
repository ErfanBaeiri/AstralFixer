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


var editorsArray = [];
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
                        editorsArray.push(editor);
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

function SubmitTagForm() {

    $("#filter_form").submit();
}

function SubmitFilterFormPagination(pageId) {
    $('#CurrentPage').val(pageId);
    $("#filter_form").submit();
}

function AnswerQuestionFormDone(response) {
    EndLoading('#submit-comment');
    console.log(response);
    if (response.status === "Success") {
        swal("اعلان", "پاسخ شما با موفقیت ثبت شد", "success");

        $("#AnswersBox").load(location.href + " #AnswersBox")

        $('html, body').animate({
            scrollTop: $("#AnswersBox").offset().top
        }, 1000);
    }
    else if (response.status === "EmptyAnswer") {
        swal("اعلان", "متن پاسخ شما نمی تواند خالی باشد", "warning");
    }
    else if (response.status === "Error") {
        swal("اعلان", "خطایی رخ داده است لطفا مجدد تلاش نمایید", "error");
    }

    for (var editor of editorsArray) {

        editor.setData('');
    }

}

function selectTrueAnswer(answerId) {
   /* var token = $("input[name=__RequestVerificationToken]").val();*/
    $.ajax({
        url: "/SelectTrueAnswer",
        type: "POST",
        data: {
            answerId: answerId,
        /*    __RequestVerificationToken = token*/
        },
        beforeSend: function () {
            StartLoading();
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#AnswersBox").load(location.href + " #AnswersBox")
            }
            else if (response.status === "NotAccess") {
                swal("اعلان", "امکان ویرایش برای شما میسر نمیباشد", "info");
            }
            else if (response.status === "NotAuthenticated") {
                swal("اعلان", "ابتدا وارد حساب کاربری خود شوید", "warning");
            }
        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    });
}

function ScoreUpForAnswer(answerId) {
    $.ajax({
        url: "/ScoreUpForAnswer",
        type: "POST",
        data: {
            answerId: answerId
        },
        beforeSend: function () {
            StartLoading();
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#AnswersBox").load(location.href + " #AnswersBox")
            }
            else if (response.status === "Error") {
                swal("خطا", "عملیات با خطا مواجه شد", "error");
            }
            else if (response.status === "NotEnoughScoreForDown") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "NotEnoughScoreForUp") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "UserCreateScoreBefore") {
                swal("اعلان", "امتیاز شما برای این پاسخ ثبت گردیده است", "info");
            }
            else if (response.status === "UserDontLogged") {
                swal("اعلان", "برای ثبت امتیاز ابتدا وارد حساب کاربری خود شوید", "info");
            }

        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    });
};

function ScoreDownForAnswer(answerId) {
    $.ajax({
        url: "/ScoreDownForAnswer",
        type: "POST",
        data: {
            answerId: answerId
        },
        beforeSend: function () {
            StartLoading();
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#AnswersBox").load(location.href + " #AnswersBox")
            }
            else if (response.status === "Error") {
                swal("خطا", "عملیات با خطا مواجه شد", "error");
            }
            else if (response.status === "NotEnoughScoreForDown") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "NotEnoughScoreForUp") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "UserCreateScoreBefore") {
                swal("اعلان", "امتیاز شما برای این پاسخ ثبت گردیده است", "info");
            }
            else if (response.status === "UserDontLogged") {
                swal("اعلان", "برای ثبت امتیاز ابتدا وارد حساب کاربری خود شوید", "info");
            }
        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    });
}

function ScoreUpForQuestion(questionId) {
    $.ajax({
        type:"POST",
        url: "/ScoreQuestionPlus",
        data: {
            questionId: questionId
        },
        beforeSend: function () {
            StartLoading()
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#QuestionScore").load(location.href + " #QuestionScore")
            }
            else if (response.status === "Error") {
                swal("خطا", "عملیات با خطا مواجه شد", "error");
            }
            else if (response.status === "NotEnoughScoreForDown") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "NotEnoughScoreForUp") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "UserCreateScoreBefore") {
                swal("اعلان", "امتیاز شما برای این پاسخ ثبت گردیده است", "info");
            }
            else if (response.status === "UserDontLogged") {
                swal("اعلان", "برای ثبت امتیاز ابتدا وارد حساب کاربری خود شوید", "info");
            }

        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    })
}

function ScoreDownForQuestion(questionId) {
    $.ajax({
        type: "POST",
        url: "/ScoreQuestionMinus",
        data: {
            questionId: questionId
        },
        beforeSend: function () {
            StartLoading()
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#QuestionScore").load(location.href + " #QuestionScore")
            }
            else if (response.status === "Error") {
                swal("خطا", "عملیات با خطا مواجه شد", "error");
            }
            else if (response.status === "NotEnoughScoreForDown") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "NotEnoughScoreForUp") {
                swal("اعلان", "امتیاز شما برای انجام این عملیات امکان پذیر نمیباشد", "warning");
            }
            else if (response.status === "UserCreateScoreBefore") {
                swal("اعلان", "امتیاز شما برای این پاسخ ثبت گردیده است", "info");
            }
            else if (response.status === "UserDontLogged") {
                swal("اعلان", "برای ثبت امتیاز ابتدا وارد حساب کاربری خود شوید", "info");
            }

        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    })
}

function AddQuestionToBookMark(questionId) {
    $.ajax({
        url: "/AddQueestionToBookMark",
        type: "POST",
        data: {
            questionId: questionId
        },
        beforeSend: function () {
            StartLoading();
        },
        success: function (response) {
            EndLoading();
            if (response.status === "Success") {
                swal("اعلان", "عملیات با موفقیت انجام شد", "success");
                $("#questionReload").load(location.href + " #questionReload")
            }
            else if (response.status === "Error") {
                swal("اعلان", "خطا لطفا با پیشتیبانی در تملس بگیرید", "warning");
            }
            else if (response.status === "NotAuthorized") {
                swal("اعلان", "ابتدا وارد حساب کاربری خود شوید", "info");
            }
        },
        error: function () {
            EndLoading();
            swal("خطا", "عملیات با خطا مواجه شد", "error");
        }
    });
}

function LoadUrl(url) {
    location.href = url;
}
