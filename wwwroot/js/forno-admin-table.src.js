import {
  createTable,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
} from "@tanstack/table-core";

function flexRender(comp, context) {
  if (typeof comp === "function") {
    return comp(context);
  }
  return comp;
}

function esc(value) {
  return String(value ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function stampClass(status) {
  return "is-" + String(status || "").replace(/_/g, "-");
}

function formatTime(value) {
  var d = new Date(value);
  if (Number.isNaN(d.getTime())) {
    return "—";
  }
  var dd = String(d.getDate()).padStart(2, "0");
  var mm = String(d.getMonth() + 1).padStart(2, "0");
  var hh = String(d.getHours()).padStart(2, "0");
  var mi = String(d.getMinutes()).padStart(2, "0");
  return dd + "." + mm + ". " + hh + ":" + mi;
}

function formatMoney(value) {
  var n = Number(value);
  return (
    n.toLocaleString("sk-SK", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }) + " €"
  );
}

function columns() {
  return [
    {
      id: "id",
      accessorKey: "id",
      header: "#",
      cell: function (info) {
        return String(info.getValue()).padStart(3, "0");
      },
    },
    {
      id: "createdAt",
      accessorKey: "createdAt",
      header: "Čas",
      cell: function (info) {
        return formatTime(info.getValue());
      },
    },
    {
      id: "name",
      accessorKey: "name",
      header: "Meno",
      cell: function (info) {
        var row = info.row.original;
        return (
          "<strong>" +
          esc(row.name) +
          '</strong><span class="admin-sub">' +
          esc(row.phone) +
          "</span>"
        );
      },
    },
    {
      id: "fulfillment",
      accessorKey: "fulfillment",
      header: "Spôsob",
      cell: function (info) {
        return String(info.getValue()).toLowerCase() === "pickup"
          ? "Výdaj"
          : "Rozvoz";
      },
    },
    {
      id: "status",
      accessorKey: "status",
      header: "Stav",
      cell: function (info) {
        var status = info.getValue();
        var label = info.row.original.statusLabel || status;
        return (
          '<span class="admin-stamp ' +
          stampClass(status) +
          '">' +
          esc(label) +
          "</span>"
        );
      },
    },
    {
      id: "total",
      accessorKey: "total",
      header: "Suma",
      cell: function (info) {
        return formatMoney(info.getValue());
      },
    },
  ];
}

function sortMark(header) {
  if (!header.column.getCanSort()) {
    return "";
  }
  var sorted = header.column.getIsSorted();
  if (sorted === "asc") {
    return " ↑";
  }
  if (sorted === "desc") {
    return " ↓";
  }
  return " ↕";
}

function createInstance(host, options) {
  var data = Array.isArray(options && options.rows) ? options.rows : [];
  var sorting = [{ id: "createdAt", desc: true }];
  var globalFilter = "";
  var pagination = {
    pageIndex: 0,
    pageSize: Math.max(5, Number(options && options.pageSize) || 10),
  };

  var root = document.createElement("div");
  root.className = "admin-tanstack";
  host.replaceChildren(root);

  function tableOptions() {
    return {
      data: data,
      columns: columns(),
      state: {
        sorting: sorting,
        globalFilter: globalFilter,
        pagination: pagination,
      },
      onSortingChange: function (updater) {
        sorting = typeof updater === "function" ? updater(sorting) : updater;
        render();
      },
      onGlobalFilterChange: function (updater) {
        globalFilter =
          typeof updater === "function" ? updater(globalFilter) : updater;
        pagination = Object.assign({}, pagination, { pageIndex: 0 });
        render();
      },
      onPaginationChange: function (updater) {
        pagination =
          typeof updater === "function" ? updater(pagination) : updater;
        render();
      },
      getCoreRowModel: getCoreRowModel(),
      getSortedRowModel: getSortedRowModel(),
      getFilteredRowModel: getFilteredRowModel(),
      getPaginationRowModel: getPaginationRowModel(),
      globalFilterFn: function (row, _columnId, filterValue) {
        var q = String(filterValue == null ? "" : filterValue)
          .trim()
          .toLowerCase();
        if (!q) {
          return true;
        }
        return String(row.original.search || "")
          .toLowerCase()
          .includes(q);
      },
      getRowId: function (row) {
        return String(row.id);
      },
    };
  }

  var table = createTable(tableOptions());

  function bind() {
    var search = root.querySelector("[data-admin-search]");
    if (search) {
      search.addEventListener("input", function (event) {
        table.setGlobalFilter(event.target.value || "");
      });
    }

    root.querySelectorAll("th[data-col]").forEach(function (th) {
      th.addEventListener("click", function () {
        var id = th.getAttribute("data-col");
        var group = table.getHeaderGroups()[0];
        var header = group
          ? group.headers.find(function (h) {
              return h.column.id === id;
            })
          : null;
        if (header) {
          header.column.toggleSorting();
        }
      });
    });

    root.querySelectorAll("tr[data-id]").forEach(function (tr) {
      tr.addEventListener("click", function (event) {
        if (event.target.closest("a")) {
          return;
        }
        var id = tr.getAttribute("data-id");
        if (id) {
          window.location.href = "/admin/orders/" + id;
        }
      });
    });

    var prev = root.querySelector("[data-prev]");
    if (prev) {
      prev.addEventListener("click", function () {
        table.previousPage();
      });
    }
    var next = root.querySelector("[data-next]");
    if (next) {
      next.addEventListener("click", function () {
        table.nextPage();
      });
    }
    var size = root.querySelector("[data-size]");
    if (size) {
      size.addEventListener("change", function (event) {
        table.setPageSize(Number(event.target.value) || 10);
      });
    }
  }

  function render() {
    table.setOptions(function (prev) {
      return Object.assign({}, prev, tableOptions());
    });

    var headers = (table.getHeaderGroups()[0] || {}).headers || [];
    var rows = table.getRowModel().rows;
    var pageCount = table.getPageCount();
    var canPrev = table.getCanPreviousPage();
    var canNext = table.getCanNextPage();
    var filtered = table.getFilteredRowModel().rows.length;

    var head = headers
      .map(function (header) {
        var label = flexRender(
          header.column.columnDef.header,
          header.getContext()
        );
        var canSort = header.column.getCanSort();
        return (
          '<th data-col="' +
          esc(header.column.id) +
          '" class="' +
          (canSort ? "is-sortable" : "") +
          '" scope="col">' +
          esc(label) +
          sortMark(header) +
          "</th>"
        );
      })
      .join("");

    var body;
    if (rows.length === 0) {
      body =
        '<tr><td colspan="' +
        headers.length +
        '" class="admin-muted">Žiadne lístky pre filter.</td></tr>';
    } else {
      body = rows
        .map(function (row) {
          var cells = row
            .getVisibleCells()
            .map(function (cell) {
              var html = flexRender(
                cell.column.columnDef.cell,
                cell.getContext()
              );
              if (cell.column.id === "id") {
                return (
                  '<td><a href="/admin/orders/' +
                  esc(row.original.id) +
                  '">' +
                  esc(html) +
                  "</a></td>"
                );
              }
              if (cell.column.id === "name" || cell.column.id === "status") {
                return "<td>" + html + "</td>";
              }
              return "<td>" + esc(html) + "</td>";
            })
            .join("");
          return (
            '<tr data-id="' + esc(row.original.id) + '">' + cells + "</tr>"
          );
        })
        .join("");
    }

    var sizeOptions = [5, 10, 20, 50]
      .map(function (n) {
        return (
          '<option value="' +
          n +
          '"' +
          (pagination.pageSize === n ? " selected" : "") +
          ">" +
          n +
          "</option>"
        );
      })
      .join("");

    root.innerHTML =
      '<div class="admin-tanstack-toolbar">' +
      '<label class="admin-tanstack-search"><span>Hľadať</span>' +
      '<input data-admin-search type="search" value="' +
      esc(globalFilter) +
      '" placeholder="meno, telefón, stav…" /></label>' +
      '<p class="admin-muted">' +
      filtered +
      " lístkov</p></div>" +
      '<div class="admin-table-wrap"><table class="admin-table"><thead><tr>' +
      head +
      "</tr></thead><tbody>" +
      body +
      "</tbody></table></div>" +
      '<div class="admin-tanstack-pager">' +
      '<button type="button" class="btn" data-prev' +
      (canPrev ? "" : " disabled") +
      ">← Predošlá</button>" +
      '<span class="admin-muted">Strana ' +
      (pagination.pageIndex + 1) +
      " / " +
      Math.max(1, pageCount) +
      "</span>" +
      '<button type="button" class="btn" data-next' +
      (canNext ? "" : " disabled") +
      ">Ďalšia →</button>" +
      '<label class="admin-tanstack-size"><span>Na stránku</span><select data-size>' +
      sizeOptions +
      "</select></label></div>";

    bind();
  }

  render();

  return {
    update: function (nextRows) {
      data = Array.isArray(nextRows) ? nextRows : [];
      pagination = Object.assign({}, pagination, { pageIndex: 0 });
      render();
    },
    destroy: function () {
      host.replaceChildren();
    },
  };
}

var tables = new Map();
var seq = 0;

window.FornoAdminTable = {
  mount: function (host, options) {
    var id = "t" + ++seq;
    tables.set(id, createInstance(host, options || {}));
    return id;
  },
  update: function (id, rows) {
    var table = tables.get(id);
    if (table) {
      table.update(rows);
    }
  },
  destroy: function (id) {
    var table = tables.get(id);
    if (table) {
      table.destroy();
    }
    tables.delete(id);
  },
};
