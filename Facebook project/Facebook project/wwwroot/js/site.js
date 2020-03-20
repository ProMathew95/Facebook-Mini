// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ConfirmBlock(uniqueId, IsBLockClicked) {
    var blockSpan = 'blockSpan' + uniqueId;
    var confirmblockspan = 'confirmblockSpan' + uniqueId;
    if (IsBLockClicked) {
        $('#' + blockSpan).hide();
        $('#' + confirmblockspan).show();
    }
    else {
        $('#' + blockSpan).show();
        $('#' + confirmblockspan).hide();
    }
}



    function srch () {
        var aaa = $('#searchi').val();
        $.ajax({
            type: 'POST',
            url: "/Admin/getuser/" + aaa,


            success: function (data)
            {
                var respUserID = Object.values(data)[0];
                var respUserName = Object.values(data)[1];

                var cards = '<div class="card"> <div class="card-header">'+ respUserID +'';
                    
                cards += '</div> <div class="card-body"> <h5 class="card-title">' + respUserName + '</h5>';
                cards += '</div> </div>';
                document.querySelector("#search").insertAdjacentHTML('beforeend',cards);
            },

            error: function (ex) {
                alert("Error searching");
            }
        });
        return false;
    }

