var dataTable;

$(document).ready(function () {
    loadDataTable('GetInquiryList');
});

function loadDataTable(url) {
    dataTable = $('#tblInquiry').DataTable({
        "ajax": {
            "url": `/inquiry/${url}`
        },
        "columns": [
            { "data": "inquiryId", "width": "10%" },
            { "data": "fullName", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "email", "width": "15%" },
            {
                "data": "inquiryDate", render: function (data) {
                    return moment(data).format('DD-MMM-YYYY');
                },
                "width": "15%"
            },
            {
                "data": "inquiryId", render: function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Inquiry/Details/${data}" class="btn btn-success text-white" style="cursor: pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "5%"
            }
        ]
    });
}