(function () {
  function buildDatalist(select, datalist) {
    datalist.innerHTML = "";
    Array.from(select.options).forEach(function (option) {
      if (!option.value) return;
      var datalistOption = document.createElement("option");
      datalistOption.value = option.text.trim();
      datalist.appendChild(datalistOption);
    });
  }

  function syncInputFromSelect(select, input) {
    var selectedOption = select.options[select.selectedIndex];
    if (selectedOption && selectedOption.value) {
      input.value = selectedOption.text.trim();
    } else {
      input.value = "";
    }
  }

  function findMatchingOption(select, value) {
    var target = value.trim();
    if (!target) return null;
    return Array.from(select.options).find(function (option) {
      return option.text.trim() === target;
    });
  }

  function syncSelectFromInput(select, input, strict) {
    var enteredValue = input.value.trim();
    if (!enteredValue) {
      select.value = "";
      return;
    }
    var match = findMatchingOption(select, enteredValue);
    if (match) {
      select.value = match.value;
      return;
    }
    if (strict) {
      select.value = "";
    }
  }

  function initCombobox(select) {
    if (select.dataset.comboboxInitialized === "true") return;
    select.dataset.comboboxInitialized = "true";

    var input = document.createElement("input");
    input.type = "text";
    input.className = "form-control";
    input.id = select.id + "_combobox";
    input.placeholder = select.dataset.comboboxPlaceholder || "請輸入搜尋";

    var datalist = document.createElement("datalist");
    datalist.id = select.id + "_datalist";
    input.setAttribute("list", datalist.id);

    select.classList.add("d-none");
    select.tabIndex = -1;

    select.parentNode.insertBefore(input, select);
    select.parentNode.insertBefore(datalist, select);
    var label = document.querySelector("label[for='" + select.id + "']");
    if (label) {
      label.setAttribute("for", input.id);
    }

    buildDatalist(select, datalist);
    syncInputFromSelect(select, input);

    input.addEventListener("input", function () {
      syncSelectFromInput(select, input, false);
    });
    input.addEventListener("change", function () {
      syncSelectFromInput(select, input, true);
    });
    select.addEventListener("change", function () {
      syncInputFromSelect(select, input);
    });

    var observer = new MutationObserver(function () {
      buildDatalist(select, datalist);
      syncInputFromSelect(select, input);
    });
    observer.observe(select, { childList: true, subtree: true });
  }

  function initComboboxes() {
    document.querySelectorAll("select[data-combobox='species']").forEach(initCombobox);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", initComboboxes);
  } else {
    initComboboxes();
  }

  window.initComboboxes = initComboboxes;
})();
