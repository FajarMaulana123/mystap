$(document).ready(function () {

    //$.ajaxSetup({
    //    headers: {
    //        'X-CSRF-TOKEN': $('meta[name="csrf-token"]').attr('content')
    //    }
    //});

    var table = $('#table').DataTable({
        dom: '<"dataTables_wrapper dt-bootstrap"<"row"<"col-xl-7 d-block d-sm-flex d-xl-block justify-content-center"<"d-block d-lg-inline-flex me-0 me-md-3"l><"d-block d-lg-inline-flex"B>><"col-xl-5 d-flex d-xl-block justify-content-center"fr>>t<"row"<"col-md-5"i><"col-md-7"p>>>',
        processing: true,
        serverSide: true,
        //deferLoading: 0,
        //language: {
        //    "emptyTable": "Data tidak ditemukan - Silahkan Filter data Job List terlebih dahulu !"
        //},
        "lengthMenu": [
            [30, 60, 100, 200, -1],
            [30, 60, 100, 200, "All"]
        ],
        ajax: {
            "url": "joblist_",
            "method": "POST",
            "datatype": "json"
            //data: function (d) {
            //    d.project = $('#project_filter').val();
            //    d.project_rev = $('#project_filter').find(':selected').data('rev');
            //    d.jobNo = $('#jobNo_filter').val();
            //    d.eqTagNo = $('#eqTagNo_filter').val();
            //    d.unitCode = $('#unitCode_filter').val();
            //    d.user_section = $('#user_section_filter').val();
            //}
        },
        columns: [
            //{ data: 'DT_RowIndex', name: 'DT_RowIndex', orderable: false, searchable: false },
            { data: 'jobNo', name: 'jobNo' },
            { data: 'projectNo', name: 'projectNo' },
            { data: 'description', name: 'project.description' },
            { data: 'nama_unit', name: 'unit.unitCode' },
            { data: 'eqTagNo', name: 'eqTagNo' },
            {
                data: 'status_tagno', name: 'status_tagno',
                render: function (data, type, full, meta) {
                    
                    if (full.status_tagno == 'ready') {
                        full.status_tagno = '<span class="badge bg-success">READY</span>';
                    } else if (full.status_tagno == 'not_ready') {
                        full.status_tagno = '<span class="badge bg-danger">NOT READY</span>';
                    } else {
                        full.status_tagno = '<span class="badge bg-secondary">UNDEFINED</span>';
                    } return full.status_tagno;

                },
            },
           
            { data: 'userSection', name: 'userSection' },
            { data: 'keterangan', name: 'keterangan' },
            { data: 'name', name: 'users.name' },
            {
                "render": function (data, type, full, meta) {
                    return '<div class="d-flex"><a href="javascript:void(0);" class="btn btn-warning  btn-xs edit mr-1"  data-id="' + full.id + '" data-alasan="' + full.alasan + '" data-jobdesc="' + full.jobDesc + '" data-engineer="' + full.engineer + '" data-revision="' + full.revision + '" data-execution="' + full.execution + '" data-responsibility="' + full.responsibility + '" data-ram="' + full.ram + '" data-notif="' + full.notif + '" data-cleaning="' + full.cleaning + '" data-inspection="' + full.inspection + '" data-repair="' + full.repair + '" data-replace="' + full.replace + '" data-ndt="' + full.ndt + '" data-modif="' + full.modif + '" data-tein="' + full.tein + '" data-coc="' + full.coc + '" data-drawing="' + full.drawing + '" data-measurement="' + full.measurement + '" data-hsse="' + full.hsse + '" data-reliability="' + full.reliability + '" data-losses="' + full.losses + '" data-energi="' + full.energi + '" data-disiplin="' + full.disiplin + '" data-project="' + full.project + '" data-critical_job="'+ full.critical_job + '" data-freezing="' + full.freezing + '" ><i class="fas fa-pen fa-xs"></i></a><a href = "javascript:void(0);" style = "margin-left:5px" class="btn btn-danger btn-xs delete " data-id="' + full.id + '" > <i class="fas fa-trash fa-xs"></i></a ></div > ';
                },
                orderable: false,
                searchable: false
            },
        ],
        //columnDefs: [
        //    { className: 'text-center', targets: [6, 7] },
        //    (user_auth == 'user') ? { "visible": false, "targets": [10] } : {},
        //],
        buttons: /*(user_auth == 'superadmin' || user_auth == 'admin') ?*/ [{
            text: '<i class="far fa-edit"></i> New',
            className: 'btn btn-success',
            action: function (e, dt, node, config) {
                window.location.href = 'create_joblist';
            }
        },
        {
            extend: 'excel',
            title: 'Job List',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }] /*: [{
            extend: 'excel',
            title: 'Job List',
            className: 'btn',
            text: '<i class="far fa-file-code"></i> Excel',
            titleAttr: 'Excel',
            exportOptions: {
                columns: ':not(:last-child)',
            }
        }]*/

    });


});