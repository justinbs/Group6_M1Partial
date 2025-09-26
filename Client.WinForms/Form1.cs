using Client.WinForms.Models;
using Client.WinForms.Services;

namespace Client.WinForms;

public partial class Form1 : Form
{
    // Controls (declared here, built at runtime)
    private TextBox txtName = null!;
    private TextBox txtCode = null!;
    private TextBox txtBrand = null!;
    private NumericUpDown numUnitPrice = null!;
    private Button btnAdd = null!;
    private Button btnUpdate = null!;
    private Button btnDelete = null!;
    private TextBox txtSearch = null!;
    private DataGridView gridItems = null!;

    private readonly ItemApi _api = new(new HttpClient());
    private List<Item> _items = new();

    // ---------- UI BUILD ----------
    private void BuildLayout()
    {
        // root layout
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // inputs
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // buttons
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));           // search
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));      // grid
        Controls.Add(layout);

        // inputs row (Name, Code, Brand, Price)
        var rowInputs = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 8
        };
        for (int i = 0; i < 8; i++)
            rowInputs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));

        var lblName = new Label { Text = "Name*", AutoSize = true, Anchor = AnchorStyles.Left };
        var lblCode = new Label { Text = "Code*", AutoSize = true, Anchor = AnchorStyles.Left };
        var lblBrand = new Label { Text = "Brand", AutoSize = true, Anchor = AnchorStyles.Left };
        var lblPrice = new Label { Text = "Unit Price (₱)*", AutoSize = true, Anchor = AnchorStyles.Left };

        txtName = new TextBox { Dock = DockStyle.Fill };
        txtCode = new TextBox { Dock = DockStyle.Fill };
        txtBrand = new TextBox { Dock = DockStyle.Fill };
        numUnitPrice = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 2, Maximum = 100000000 };

        rowInputs.Controls.Add(lblName, 0, 0);
        rowInputs.Controls.Add(txtName, 1, 0);
        rowInputs.Controls.Add(lblCode, 2, 0);
        rowInputs.Controls.Add(txtCode, 3, 0);
        rowInputs.Controls.Add(lblBrand, 4, 0);
        rowInputs.Controls.Add(txtBrand, 5, 0);
        rowInputs.Controls.Add(lblPrice, 6, 0);
        rowInputs.Controls.Add(numUnitPrice, 7, 0);

        layout.Controls.Add(rowInputs, 0, 0);

        // buttons row
        var rowButtons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
        btnAdd = new Button { Text = "Add", AutoSize = true, Margin = new Padding(3, 6, 3, 6) };
        btnUpdate = new Button { Text = "Update", AutoSize = true, Margin = new Padding(3, 6, 3, 6) };
        btnDelete = new Button { Text = "Delete", AutoSize = true, Margin = new Padding(3, 6, 3, 6) };
        rowButtons.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete });

        layout.Controls.Add(rowButtons, 0, 1);

        // search row
        var rowSearch = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2 };
        rowSearch.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        rowSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        var lblSearch = new Label { Text = "Search", AutoSize = true, Anchor = AnchorStyles.Left };
        txtSearch = new TextBox { Dock = DockStyle.Fill };
        rowSearch.Controls.Add(lblSearch, 0, 0);
        rowSearch.Controls.Add(txtSearch, 1, 0);

        layout.Controls.Add(rowSearch, 0, 2);

        // grid
        gridItems = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        layout.Controls.Add(gridItems, 0, 3);
    }
    public Form1()
    {
        InitializeComponent();
        BuildLayout();
        StyleUi();
        WireEvents();
    }
    private void StyleUi()
    {
        // Form look
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(252, 247, 250);   // light blush

        // Buttons
        foreach (var b in new[] { btnAdd, btnUpdate, btnDelete })
        {
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Padding = new Padding(14, 6, 14, 6);
            b.BackColor = Color.FromArgb(240, 232, 245);
        }
        btnAdd.BackColor = Color.FromArgb(210, 232, 255);
        btnUpdate.BackColor = Color.FromArgb(222, 245, 216);
        btnDelete.BackColor = Color.FromArgb(255, 224, 224);

        // Inputs
        foreach (var tb in new[] { txtName, txtCode, txtBrand, txtSearch })
            tb.BorderStyle = BorderStyle.FixedSingle;

        numUnitPrice.BorderStyle = BorderStyle.FixedSingle;

        // DataGridView polish
        gridItems.BackgroundColor = Color.White;
        gridItems.BorderStyle = BorderStyle.None;
        gridItems.EnableHeadersVisualStyles = false;
        gridItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(239, 235, 244);
        gridItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        gridItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
        gridItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 231, 255);
        gridItems.DefaultCellStyle.SelectionForeColor = Color.Black;
        gridItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 253);
        gridItems.RowHeadersVisible = false;
        gridItems.GridColor = Color.FromArgb(230, 230, 240);
    }

    private void WireEvents()
    {
        Load += async (_, __) => await ReloadAsync();
        btnAdd.Click += async (_, __) => await AddAsync();
        btnUpdate.Click += async (_, __) => await UpdateAsync();
        btnDelete.Click += async (_, __) => await DeleteAsync();
        txtSearch.TextChanged += (_, __) => ApplyFilter();
        gridItems.SelectionChanged += (_, __) => BindSelection();
    }

    // ---------- DATA/UI LOGIC ----------
    private async Task ReloadAsync()
    {
        try
        {
            _items = await _api.GetAllAsync() ?? new();
            BindGrid(_items);
            BindSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load items:\n{ex.Message}");
        }
    }

    private void BindGrid(IEnumerable<Item> data)
    {
        gridItems.DataSource = data
            .Select(x => new {
                x.Id,
                x.Name,
                x.Code,
                x.Brand,
                UnitPrice = x.UnitPrice.ToString("C") // culture-aware currency
            })
            .ToList();

        // Friendly headers (in case auto-generation uses property names)
        foreach (DataGridViewColumn c in gridItems.Columns)
        {
            if (c.Name == "Id") c.HeaderText = "ID";
            if (c.Name == "Name") c.HeaderText = "Name";
            if (c.Name == "Code") c.HeaderText = "Code";
            if (c.Name == "Brand") c.HeaderText = "Brand";
            if (c.Name == "UnitPrice") c.HeaderText = "Unit Price";
        }
    }

    private void BindSelection()
    {
        if (gridItems.CurrentRow?.Cells["Id"]?.Value is int id)
        {
            var it = _items.FirstOrDefault(i => i.Id == id);
            if (it != null)
            {
                txtName.Text = it.Name;
                txtCode.Text = it.Code;
                txtBrand.Text = it.Brand ?? "";
                numUnitPrice.Value = it.UnitPrice;
                return;
            }
        }
        ClearInputs();
    }

    private void ClearInputs()
    {
        txtName.Text = txtCode.Text = txtBrand.Text = "";
        numUnitPrice.Value = 0;
    }

    private void ApplyFilter()
    {
        var s = (txtSearch.Text ?? "").ToLowerInvariant();
        var filtered = _items.Where(i =>
            (i.Name ?? "").ToLower().Contains(s) ||
            (i.Code ?? "").ToLower().Contains(s) ||
            (i.Brand ?? "").ToLower().Contains(s)
        );
        BindGrid(filtered);
    }

    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCode.Text))
        { MessageBox.Show("Name and Code are required."); return; }

        try
        {
            await _api.CreateAsync(new Item
            {
                Name = txtName.Text.Trim(),
                Code = txtCode.Text.Trim(),
                Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim(),
                UnitPrice = numUnitPrice.Value
            });
            await ReloadAsync();
            ClearInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add:\n{ex.Message}");
        }
    }

    private async Task UpdateAsync()
    {
        if (gridItems.CurrentRow?.Cells["Id"]?.Value is not int id) return;

        try
        {
            await _api.UpdateAsync(id, new Item
            {
                Name = txtName.Text.Trim(),
                Code = txtCode.Text.Trim(),
                Brand = string.IsNullOrWhiteSpace(txtBrand.Text) ? null : txtBrand.Text.Trim(),
                UnitPrice = numUnitPrice.Value
            });
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update:\n{ex.Message}");
        }
    }

    private async Task DeleteAsync()
    {
        if (gridItems.CurrentRow?.Cells["Id"]?.Value is not int id) return;
        if (MessageBox.Show("Delete selected item?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

        try
        {
            await _api.DeleteAsync(id);
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete:\n{ex.Message}");
        }
    }
}
