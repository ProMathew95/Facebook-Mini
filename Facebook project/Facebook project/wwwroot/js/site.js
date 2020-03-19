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
