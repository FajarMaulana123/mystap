$(document).ready(function () {
    $(".select2").select2({});
    $('#Modal').on('show.bs.modal', function (event) {
        $(document).ready(function () {
            $('.search_dropdown').select2({
                dropdownParent: $('#Modal')
            });
        });
    });
    $.ajaxSetup({
        headers: {
            'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
        }
    });

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        ajax: {
            url: '/equipments_',
            method: 'POST',
        },

        columns: [
            /* { data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },*/
            { "data": 'DT_RowIndex', "name": 'DT_RowIndex', searchable: false },
           /* {
                "data": null, orderable: false, "render": function (data, type, full, meta) {
                    return meta.row + 1;
                }
            },*/
            { data: 'eqTagNo', name: 'eqTagNo' },
            { data: 'eqDesc', name: 'eqDesc' },
            { data: 'funcLocID', name: 'funcLocID' },
            { data: 'planner_group', name: 'planner_group' },
            { data: 'main_work_center', name: 'main_work_center' },
            { data: 'location', name: 'location' },
            { data: 'craft', name: 'craft' },
             {
                "render": function (data, type, full, meta) {
                     return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1" data-id="' + full.id + '" data-eqtagno="' + full.eqTagNo + '" data-description="' + full.eqDesc + '" data-func_location="' + full.funcLocID + '" data-weight="' + full.weight + '" data-jenis_weight="' + full.weight_unit + '" data-size="' + full.size + '" data-start_up_date="' + full.start_up_date + '" data-acquisition_value="' + full.acquisition_value + '" data-date_acquisition="' + full.acquisition_date + '" data-currency_key="' + full.currency_key + '" data-planning_plant="' + full.planning_plant + '" data-planning_group="' + full.planner_group + '" data-main_work_center="' + full.main_work_center + '" data-catalog_profile="' + full.catalog_profile + '" data-main_plant="' + full.maint_plant + '" data-location="' + full.location + '" data-plant_section="' + full.plant_section + '" data-main_asset_no="' + full.main_asset_no + '" data-asset_sub_no="' + full.asset_sub_no + '" data-cost_center="' + full.cost_center + '" data-wbselement="' + full.WBS_element + '" data-position="' + full.Position + '" data-tin="' + full.tin + '" data-manufacturer="' + full.manufacturer + '" data-model="' + full.model + '" data-part_no="' + full.part_no + '" data-serial_no="' + full.serial_no + '" data-equipment_category="' + full.eqp_cat + '" data-date_validation="' + full.date_valid + '" data-object_type="' + full.object_type + '" data-craft="' + full.craft + '" data-country_of_manufacture="' + full.country_of_manuf + '" data-unit_proses="' + full.unitProses + '" data-year_const="' + full.year_of_const + '" data-unit_kilang="' + full.unitKilang + '" data-month_const="' + full.month_of_const + '" data-plant_main_work_center="' + full.plant_main_work_center + '" data-const_type="' + full.const_type + '" data-premit_assign="' + full.permit_assign + '" data-critical="' + full.Criticallity + '" data-remark="' + full.Remark + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="'+full.id+'" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },

        ],

        //columnDefs: [
        //    (user_auth == 'user') ? { "visible": false, "targets": [8] } : {},
        //],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ? */[{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                $('#add-form')[0].reset();
                $('#Modal').modal('show');
                $('#btn-sb').text('Tambah');
                $('.judul-modal').text('Tambah Equipment');
                $('#hidden_status').val('add');
            }
        },

        {
            extend: 'excel',
            title: 'Equipment',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        },] /*: [{
            extend: 'excel',
            title: 'Equipment',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/
    });


    // Here we create the index column in jquery datatable

    const table = new DataTable('#table', {
        columnDefs: [
            {
                searchable: false,
                orderable: false,
                targets: 0
            }
        ],
        order: [[1, 'asc']]
    });

    table
        .on('order.dt search.dt', function () {
            let i = 1;

            table
                .cells(null, 0, { search: 'applied', order: 'applied' })
                .every(function (cell) {
                    this.data(i++);
                });
        })
        .draw();
    $(document).on('click', '.edit', function () {
        $('#add-form')[0].reset();
        $('#Modal').modal('show');
        $('.judul-modal').text('Edit Equipment');
        $('#hidden_status').val('edit');
        $('#hidden_id').val($(this).data('id'));
        $('#eqtagno').val($(this).data('eqtagno'));
        $('#description').val($(this).data('description'));
        $('#func_location').val($(this).data('func_location'));
        $('#weight').val($(this).data('weight'));
        $('#jenis_weight').val($(this).data('jenis_weight'));
        $('#size').val($(this).data('size'));
        $('#start_up_date').val($(this).data('start_up_date'));
        $('#acquisition_value').val($(this).data('acquisition_value'));
        $('#date_acquisition').val($(this).data('date_acquisition'));
        $('#currency_key').val($(this).data('currency_key'));
        $('#planning_plant').val($(this).data('planning_plant'));
        $('#planning_group').val($(this).data('planning_group'));
        $('#main_work_center').val($(this).data('main_work_center'));
        $('#catalog_profile').val($(this).data('catalog_profile'));
        $('#main_plant').val($(this).data('main_plant'));
        $('#location').val($(this).data('location'));
        $('#plant_section').val($(this).data('plant_section'));
        $('#main_asset_no').val($(this).data('main_asset_no'));
        $('#asset_sub_no').val($(this).data('asset_sub_no'));
        $('#cost_center').val($(this).data('cost_center'));
        $('#wbselement').val($(this).data('wbsElement'));
        $('#position').val($(this).data('position'));
        $('#tin').val($(this).data('tin'));
        $('#manufacturer').val($(this).data('manufacturer'));
        $('#model').val($(this).data('model'));
        $('#part_no').val($(this).data('part_no'));
        $('#serial_no').val($(this).data('serial_no'));
        $('#equipment_category').val($(this).data('equipment_category'));
        $('#date_validation').val($(this).data('date_validation'));
        $('#object_type').val($(this).data('object_type'));
        $('#craft').val($(this).data('craft'));
        $('#country_of_manufacture').val($(this).data('country_of_manufacture'));
        $('#unit_proses').val($(this).data('unit_proses'));
        $('#year_const').val($(this).data('year_const'));
        $('#unit_kilang').val($(this).data('unit_kilang'));
        $('#month_const').val($(this).data('month_const'));
        $('#plant_main_work_center').val($(this).data('plant_main_work_center'));
        $('#const_type').val($(this).data('const_type'));
        $('#premit_assign').val($(this).data('premit_assign'));
        $('#critical').val($(this).data('critical'));
        $('#remark').val($(this).data('remark'));
    });


    $(document).on('click', '.delete', function () {
        var id = $(this).data('id');
        Swal.fire({
            title: 'Apakah Anda Yakin?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            confirmButtonText: 'Ya, Hapus!',
            cancelButtonText: 'Tidak',
        }).then((result) => {
            if (result.value) {
                $.ajax({
                    url: "/delete_equipment",
                    type: "POST",
                    data: {
                        id: id
                    },
                    dataType: "JSON",
                    success: function (data) {
                        table.ajax.reload();
                        Swal.fire({
                            title: data.title,
                            text: data.status,
                            type: data.icon,
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        alert('Error');
                    }
                });
            }
        })
    });

    $(document).on('change', '#unit_proses', function () {
        $.ajax({
            url: "/getUnitKilang",
            method: "POST",
            data: {
                unitCode: $(this).val(),
            },
            success: function (res) {
                $('#unit_kilang').html(res);
            }
        })
    })

    $("#add-form").validate({
        errorClass: "is-invalid",
        rules: {
            eqtagno: {
                required: true
            }

        },
        submitHandler: function (form) {
            let url;
            if ($('#hidden_status').val() == 'add') {
                url = '/create_equipment';
            } else {
                url = '/update_equipment';
            }
            $.ajax({
                url: url,
                type: "POST",
                data: new FormData(document.getElementById("add-form")),
                dataType: "JSON",
                contentType: false,
                cache: false,
                processData: false,
                beforeSend: function () {
                    swal.fire({
                        title: 'Harap Tunggu!',
                        allowEscapeKey: false,
                        allowOutsideClick: false,
                        showCancelButton: false,
                        showConfirmButton: false,
                        buttons: false,
                        didOpen: () => {
                            Swal.showLoading()
                        }
                    })
                },
                success: function (data) {
                    if (data.cek != true) {
                        swal.fire({
                            title: 'Gagal',
                            text: "Gagal Equipment Number Sudah Ada !",
                            icon: 'error',
                            // timer: 3000,
                            showCancelButton: false,
                            showConfirmButton: true,
                            // buttons: false,
                        });
                    } else {
                        if (data.result != true) {
                            swal.fire({
                                title: 'Gagal',
                                text: "Gagal Tambah / Update Equipment",
                                icon: 'error',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                            table.ajax.reload();
                        } else {
                            swal.fire({
                                title: 'Berhasil',
                                icon: 'success',
                                // timer: 3000,
                                showCancelButton: false,
                                showConfirmButton: true,
                                // buttons: false,
                            });
                            $('#Modal').modal('hide');
                            table.ajax.reload();
                        }
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert('Error adding / update data');
                }
            });
        }
    });

});