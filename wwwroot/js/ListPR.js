$(document).ready(function () {

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
            url: '/list_pr_',
            method: 'POST',
            data: function (d) {
                d.project_filter = $('#project_filter').val();
                d.lldi_filter = $('#lld_filter').val();
                d.taoh_filter = $('#taoh_filter').val();
                d.eqTagNo_filter = $('#eqTagNo_filter').val();
                d.pr_filter = $('#pr_filter').val();
                d.wo_filter = $('#wo_filter').val();
            }
        },
        deferLoading: 0,
        language: {
            "emptyTable": "Data tidak ditemukan - Silahkan Filter data Outstanding Reservasi terlebih dahulu !"
        },
        "autoWidth": true,
        "lengthMenu": [
            [10, 25, 50, 100, -1],
            [10, 25, 50, 100, "All"]
        ],
        columns: [
            { data: 'pr', name: 'pr' },
            { data: 'pr_item', name: 'pr_item' },
            { data: 'qty_pr', name: 'qty_pr' },
            { data: 'pg', name: 'pg' },
            { data: 'reqmt_date', name: 'reqmt_date' },
            { data: 'order', name: 'order' },
            { data: 'material', name: 'material' },
            { data: 'material_description', name: 'material_description' },
        ],
        buttons: [

        ]
    });


    $(document).on('click', '#filter', function () {
        table.ajax.reload();
    })
});