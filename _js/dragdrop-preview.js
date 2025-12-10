$(function () {
    var $input = $('#photo');
    var $drop = $('#photoDrop');
    var $preview = $('#photoPreview');
    var dt = new DataTransfer();

    function updateInput() {
        $input[0].files = dt.files;
    }

    function addFile(file) {
        dt.items.add(file);
        var index = dt.items.length - 1;
        var reader = new FileReader();
        reader.onload = function (e) {
            var $col = $('<div class="col-3 mb-2 text-center position-relative" data-idx="' + index + '"></div>');
            var $img = $('<img class="w-100 mb-1 preview" draggable="true" />').attr('src', e.target.result);
            var $btn = $('<button type="button" class="btn-close position-absolute top-0 end-0" style="background-color:#fff" aria-label="Remove"></button>');
            function remove() {
                removeFile($col.data('idx'));
            }
            $btn.on('click', remove);
            $img.on('click', remove).on('dragstart', function (ev) {
                ev.originalEvent.dataTransfer.setData('text/plain', '');
                remove();
            });
            $col.append($img).append($btn);
            $preview.append($col);
        };
        reader.readAsDataURL(file);
        updateInput();
    }

    function removeFile(index) {
        dt.items.remove(index);
        $preview.find('[data-idx="' + index + '"]').remove();
        $preview.children().each(function (i, el) {
            $(el).attr('data-idx', i);
        });
        updateInput();
    }

    function handleFiles(files) {
        Array.from(files).forEach(function (file) {
            if (!file.type.startsWith('image/')) return;
            addFile(file);
        });
    }

    $drop.on('dragover', function (e) {
        e.preventDefault();
        $drop.addClass('dragover');
    }).on('dragleave', function (e) {
        e.preventDefault();
        $drop.removeClass('dragover');
    }).on('drop', function (e) {
        e.preventDefault();
        $drop.removeClass('dragover');
        handleFiles(e.originalEvent.dataTransfer.files);
    }).on('click', function () {
        $input.trigger('click');
    });

    $input.on('change', function () {
        handleFiles(this.files);
        this.value = '';
    });
});
