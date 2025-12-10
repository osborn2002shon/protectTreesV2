var careIndex = 0;

function createDropZone(name) {
    return '<div class="drop-area text-center">拖曳或點擊上傳<br/><img class="preview d-none" /></div>' +
        '<input type="file" name="' + name + '" class="d-none file-input" accept="image/*" />';
}

$(function () {
    function addBlock() {
        careIndex++;
        var html = '<div class="care-photo-block mb-2">' +
            '<div class="row mb-2">' +
            '<div class="col"><label>#' + careIndex + '</label>' +
            '<label>區塊名稱</label><input type="text" name="uploadPhoto[' + careIndex + '][name]" class="w-100" /></div>' +
            '<div class="col-auto d-flex align-items-end"><button type="button" class="btn_def remove-block">刪除</button></div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="col"><label>施作前</label>' + createDropZone('uploadPhoto[' + careIndex + '][before]') + '</div>' +
            '<div class="col"><label>施作後</label>' + createDropZone('uploadPhoto[' + careIndex + '][after]') + '</div>' +
            '</div>' +
            '</div>';
        $('#uploadPhotoBlocks').append(html);
    }

    $('#addUploadPhotoBlock').on('click', addBlock);
    // create one block by default
    addBlock();

    $('#uploadPhotoBlocks')
    .on('click', '.remove-block', function () {
        $(this).closest('.care-photo-block').remove();
    })
    .on('click', '.drop-area', function () {
        $(this).siblings('input[type=file]').trigger('click');
    })
    .on('change', '.file-input', function () {
        var $zone = $(this).siblings('.drop-area');
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $zone.find('img').attr('src', e.target.result).removeClass('d-none').attr('draggable', true);
            };
            reader.readAsDataURL(file);
        } else {
            $zone.find('img').addClass('d-none').attr('src', '').removeAttr('draggable');
        }
    })
    .on('dragover', '.drop-area', function (e) {
        e.preventDefault();
        $(this).addClass('dragover');
    })
    .on('dragleave drop', '.drop-area', function (e) {
        e.preventDefault();
        $(this).removeClass('dragover');
    })
    .on('drop', '.drop-area', function (e) {
        e.preventDefault();
        var files = e.originalEvent.dataTransfer.files;
        if (files.length) {
            var input = $(this).siblings('input[type=file]')[0];
            input.files = files;
            $(input).trigger('change');
        }
    })
    .on('click', '.drop-area img', function () {
        var $zone = $(this).parent();
        $zone.siblings('input[type=file]').val('');
        $(this).addClass('d-none').attr('src', '').removeAttr('draggable');
    })
    .on('dragstart', '.drop-area img', function (e) {
        e.originalEvent.dataTransfer.setData('text/plain', '');
        var $zone = $(this).parent();
        $zone.siblings('input[type=file]').val('');
        $(this).addClass('d-none').attr('src', '').removeAttr('draggable');
    });
})

