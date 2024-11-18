$(document).ready(function() {
    $("#add-form").validate({
        errorClass: "is-invalid",
        // validClass: "is-valid",
        rules: {
            username: {
                required: true
            },
            email: {
                required: true
            },
            //password: {
            //    required: function () {
            //        if ($('#hidden_status').val() == 'edit') {
            //            return false;
            //        } else {
            //            return true;
            //        }
            //    },
            //    minlength: 6
            //},
            //repassword: {
            //    required: function () {
            //        if ($('#hidden_status').val() == 'edit') {
            //            return false;
            //        } else {
            //            return true;
            //        }
            //    },
            //    minlength: 6,
            //    equalTo: "#password",
            //},
        },
        submitHandler: function (form) {
            let url;
            
            url = '/edit_profile_';
            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("add-form")),
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
                success: function (data) {
                    if (data.results != true) {
                        Swal.fire({
                            title: 'Gagal',
                            text: "Gagal Edit Profile",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    } else {
                        Swal.fire({
                            title: 'Berhasil',
                            icon: 'success',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                        window.location.href = 'profile';
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });
})