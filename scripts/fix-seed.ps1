# Fix seed script: swap POINT coordinates and add real image URLs
$content = Get-Content "C:\Users\vanil\source\repos\IOrderNew\scripts\seed-fortaleza.sql" -Raw

# 1. Swap all POINT(lon lat) to POINT(lat lon) - MySQL reads first as X=lat for SRID 4326
$content = $content -replace "POINT\(([-\d.]+) ([-\d.]+)\)", 'POINT($2 $1)'

Write-Host "Swapped all POINT coordinates"

# 2. Fix product images - replace empty '' with real Unsplash URLs
# Products section: each product has name, description, '' for ImageUrl
# We need to find the product name and assign an image

$productImages = @{
    'Bolo de Chocolate Premium' = 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=300'
    'Bolo de Morango' = 'https://images.unsplash.com/photo-1562440499-64c9a111f713?w=300'
    'Bolo de Cenoura Gourmet' = 'https://images.unsplash.com/photo-1621303837174-89787a7d4729?w=300'
    'Bolo de Limao' = 'https://images.unsplash.com/photo-1558636508-e0db3814bd1d?w=300'
    'Bolo Red Velvet' = 'https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=300'
    'Bolo de Fuba com Goiabada' = 'https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?w=300'
    'Bolo de Coco' = 'https://images.unsplash.com/photo-1614707267537-b85aaf00c4b7?w=300'
    'Bolo Formigueiro' = 'https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=300'
    'Bolo de Creme' = 'https://images.unsplash.com/photo-1550617931-e17a7b70dce2?w=300'
    'Bolo Especial do Dia' = 'https://images.unsplash.com/photo-1588195538326-c5b1e9f80a1b?w=300'
    'Trufas de Chocolate' = 'https://images.unsplash.com/photo-1551024601-bec78aea704b?w=300'
    'Brigadeiro Gourmet' = 'https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=300'
    'Beijinho de Coco' = 'https://images.unsplash.com/photo-1549007994-cb92caebd54b?w=300'
    'Cajuzinho' = 'https://images.unsplash.com/photo-1558326567-98ae2405596b?w=300'
    'Bem Casado' = 'https://images.unsplash.com/photo-1587314168485-3236d6710814?w=300'
    'Cocada Branca' = 'https://images.unsplash.com/photo-1548848221-0c2e497ed557?w=300'
    'Pacoca' = 'https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?w=300'
    'Maria Mole' = 'https://images.unsplash.com/photo-1488477181946-6428a0291777?w=300'
    'Manjar com Calda' = 'https://images.unsplash.com/photo-1606890737304-57a1ca8a5b62?w=300'
    'Doce de Abobara' = 'https://images.unsplash.com/photo-1571115177098-24ec42ed204d?w=300'
    'Coxinha de Frango' = 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=300'
    'Pastel de Carne' = 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=300'
    'Risoles de Camarao' = 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=300'
    'Esfirra de Carne' = 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300'
    'Empada de Palmito' = 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=300'
    'Quibe Assado' = 'https://images.unsplash.com/photo-1585032226651-759b368d7246?w=300'
    'Bolinho de Bacalhau' = 'https://images.unsplash.com/photo-1528735602780-2552fd46c7af?w=300'
    'Coxinha de Camarao' = 'https://images.unsplash.com/photo-1476224203421-9ac39bcb3327?w=300'
    'Croquete de Costela' = 'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=300'
    'Pizza de Liquidificador' = 'https://images.unsplash.com/photo-1563379926898-05f4575a45d8?w=300'
    'Cesta Cafe Manha' = 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=300'
    'Cesta Pao Caseiro' = 'https://images.unsplash.com/photo-1484723091739-30a097e8f929?w=300'
    'Cesta Frutas Secas' = 'https://images.unsplash.com/photo-1495147466023-ac5c588e2e94?w=300'
    'Cesta Cafe Premium' = 'https://images.unsplash.com/photo-1490474418585-ba9bad8fd0ea?w=300'
    'Cesta Padaria' = 'https://images.unsplash.com/photo-1464305795204-6f5bbfc7fb81?w=300'
    'Cesta Smoothie' = 'https://images.unsplash.com/photo-1486297678162-eb2c15f40eed?w=300'
    'Cesta Acai' = 'https://images.unsplash.com/photo-1525351484163-7529414344d8?w=300'
    'Cesta Mista' = 'https://images.unsplash.com/photo-1608198093002-ad4e005484ec?w=300'
    'Cesta Fitness' = 'https://images.unsplash.com/photo-1587049352846-4a222e784d38?w=300'
    'Cesta Infantil' = 'https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?w=300'
    'Lembrancinha Personalizada' = 'https://images.unsplash.com/photo-1513151233558-d860c5398176?w=300'
    'Lembrancinha Mini' = 'https://images.unsplash.com/photo-1513542789411-b6a5d4f31634?w=300'
    'Lembrancinha Premium' = 'https://images.unsplash.com/photo-1549465220-8d803a87c82f?w=300'
    'Kit Churrasco' = 'https://images.unsplash.com/photo-1512909006721-3d6018887383?w=300'
    'Lembrancinha Festa' = 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300'
    'Lembrancinha Casamento' = 'https://images.unsplash.com/photo-1519541856657-538a8801e402?w=300'
    'Lembrancinha Infantil' = 'https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=300'
    'Kit Cafe da Manha' = 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=300'
    'Lembrancinha Corporativa' = 'https://images.unsplash.com/photo-1547036967-23d11aacaee0?w=300'
    'Kit Presente' = 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300'
    'Marmita Fit' = 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=300'
    'Marmita Low Carb' = 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=300'
    'Marmita Vegana' = 'https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=300'
    'Marmita Proteica' = 'https://images.unsplash.com/photo-1547592180-85f173990554?w=300'
    'Marmita Detox' = 'https://images.unsplash.com/photo-1498837167922-ddd27525d352?w=300'
    'Marmita Kids' = 'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=300'
    'Marmita Bodybuilder' = 'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?w=300'
    'Marmita Vegetariana' = 'https://images.unsplash.com/photo-1563379926898-05f4575a45d8?w=300'
    'Marmita Especial do Dia' = 'https://images.unsplash.com/photo-1432139509613-5c4255a1d985?w=300'
    'Marmita Doce' = 'https://images.unsplash.com/photo-1517433367423-c7e5b0f35086?w=300'
    'Torta de Frango' = 'https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=300'
    'Torta de Palmito' = 'https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=300'
    'Torta de Presunto e Queijo' = 'https://images.unsplash.com/photo-1562440499-64c9a111f713?w=300'
    'Torta de Camarao' = 'https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?w=300'
    'Torta de Leite' = 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=300'
    'Torta de Abobara' = 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=300'
    'Torta de Bacon' = 'https://images.unsplash.com/photo-1486427944544-d2c246c4df14?w=300'
    'Empadao de Frango' = 'https://images.unsplash.com/photo-1586985289688-ca3cf47d3e6e?w=300'
    'Quiche de Legumes' = 'https://images.unsplash.com/photo-1517433367423-c7e5b0f35086?w=300'
    'Torta Recheada Especial' = 'https://images.unsplash.com/photo-1517686469429-8bdb88b9f907?w=300'
    'Boneca de Renda' = 'https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=300'
    'Cesto de Palha' = 'https://images.unsplash.com/photo-1528698827591-e19cef1a992c?w=300'
    'Barraca de Praia' = 'https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=300'
    'Ceramica Cearense' = 'https://images.unsplash.com/photo-1513519245088-0e12902e35ca?w=300'
    'Chapeu de Palha' = 'https://images.unsplash.com/photo-1460661419201-fd4cecdf8a8b?w=300'
    'Bolsa de Palha' = 'https://images.unsplash.com/photo-1519741497674-611481863552?w=300'
    'Quadro Pintado' = 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300'
    'Vaso de Ceramica' = 'https://images.unsplash.com/photo-1525351484163-7529414344d8?w=300'
    'Jogo de Mesa' = 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300'
    'Escultura de Madeira' = 'https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=300'
    'Kit Festa Infantil' = 'https://images.unsplash.com/photo-1530103862676-de8c9debad1d?w=300'
    'Kit Decoracao' = 'https://images.unsplash.com/photo-1516450360452-9312f5e86fc7?w=300'
    'Kit Confeitaria' = 'https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=300'
    'Kit Festa Junina' = 'https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?w=300'
    'Kit Casamento' = 'https://images.unsplash.com/photo-1511795409834-ef04bbd61622?w=300'
    'Kit Aniversario' = 'https://images.unsplash.com/photo-1549465220-8d803a87c82f?w=300'
    'Kit Cha Bar' = 'https://images.unsplash.com/photo-1519541856657-538a8801e402?w=300'
    'Kit Churrasco (Festa)' = 'https://images.unsplash.com/photo-1513151233558-d860c5398176?w=300'
    'Kit Formatura' = 'https://images.unsplash.com/photo-1543002588-bfa74002ed7e?w=300'
    'Kit Bebe' = 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=300'
    'Cerveja Artesanal IPA' = 'https://images.unsplash.com/photo-1532634922-8fe0b757fb13?w=300'
    'Suco Natural Laranja' = 'https://images.unsplash.com/photo-1622597467836-f3285f2131b8?w=300'
    'Kombu de Maracuja' = 'https://images.unsplash.com/photo-1544145945-f90425340c7e?w=300'
    'Chopp Artesanal' = 'https://images.unsplash.com/photo-1470337458703-46ad1756a187?w=300'
    'Cerveja Stout' = 'https://images.unsplash.com/photo-1551024709-8f23befc6f87?w=300'
    'Limonada Natural' = 'https://images.unsplash.com/photo-1536935338788-846bb9981813?w=300'
    'Vinho de Frutas' = 'https://images.unsplash.com/photo-1541546006121-e8e0acd41d0d?w=300'
    'Gelo Saborizado' = 'https://images.unsplash.com/photo-1497534446932-c925b458314e?w=300'
    'Cerveja Wheat' = 'https://images.unsplash.com/photo-1587223962930-cb7f31384c79?w=300'
    'Agua de Coco' = 'https://images.unsplash.com/photo-1498611750634-28f3faa8b0d1?w=300'
}

# Also fix store images (the store section already has images from the rebuild)
$storeImages = @{
    'Doce Encanto' = 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400'
    'Bolos & Arte' = 'https://images.unsplash.com/photo-1621303837174-89787a7d4729?w=400'
    'Confeitaria Sonho' = 'https://images.unsplash.com/photo-1562440499-64c9a111f713?w=400'
    'Boleira Real' = 'https://images.unsplash.com/photo-1558636508-e0db3814bd1d?w=400'
    'Doce Vida' = 'https://images.unsplash.com/photo-1486427944544-d2c246c4df14?w=400'
    'Sweet Dreams' = 'https://images.unsplash.com/photo-1549007994-cb92caebd54b?w=400'
    'Doces Maria' = 'https://images.unsplash.com/photo-1551024601-bec78aea704b?w=400'
    'Fondant House' = 'https://images.unsplash.com/photo-1558326567-98ae2405596b?w=400'
    'Confeitaria Fina' = 'https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=400'
    'Doce Arte' = 'https://images.unsplash.com/photo-1587314168485-3236d6710814?w=400'
    'Salgados da Tia' = 'https://images.unsplash.com/photo-1529692236671-f1f6cf9683ba?w=400'
    'Festa & Sabor' = 'https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=400'
    'Salgados Premium' = 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400'
    'Estacao Salgados' = 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400'
    'Salgados Express' = 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400'
    'Cesta & Cia' = 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400'
    'Bom Dia Cestas' = 'https://images.unsplash.com/photo-1484723091739-30a097e8f929?w=400'
    'Cafe Manha' = 'https://images.unsplash.com/photo-1495147466023-ac5c588e2e94?w=400'
    'Cestas Encantadas' = 'https://images.unsplash.com/photo-1490474418585-ba9bad8fd0ea?w=400'
    'Alegria da Manha' = 'https://images.unsplash.com/photo-1464305795204-6f5bbfc7fb81?w=400'
    'Lembrancinhas Arte' = 'https://images.unsplash.com/photo-1513151233558-d860c5398176?w=400'
    'Personaliza Ja' = 'https://images.unsplash.com/photo-1513542789411-b6a5d4f31634?w=400'
    'Memoria Doce' = 'https://images.unsplash.com/photo-1549465220-8d803a87c82f?w=400'
    'Lembrancas' = 'https://images.unsplash.com/photo-1512909006721-3d6018887383?w=400'
    'Presente Criativo' = 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=400'
    'Marmita Fit' = 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400'
    'Saude & Sabor' = 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400'
    'Green Box' = 'https://images.unsplash.com/photo-1490645935967-10de6ba17061?w=400'
    'Marmita Vida' = 'https://images.unsplash.com/photo-1547592180-85f173990554?w=400'
    'Comida Boa' = 'https://images.unsplash.com/photo-1498837167922-ddd27525d352?w=400'
    'Tortas da Nonna' = 'https://images.unsplash.com/photo-1621996346565-e3dbc646d9a9?w=400'
    'Tortas & Cia' = 'https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=400'
    'Casa da Torta' = 'https://images.unsplash.com/photo-1476718406336-bb5a9690ee2a?w=400'
    'Tortas Gourmet' = 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400'
    'Sabor da Torta' = 'https://images.unsplash.com/photo-1464305795204-6f5bbfc7fb81?w=400'
    'Artesanato Fortaleza' = 'https://images.unsplash.com/photo-1513364776144-60967b0f800f?w=400'
    'Mao de Arte' = 'https://images.unsplash.com/photo-1528698827591-e19cef1a992c?w=400'
    'Criatividade Total' = 'https://images.unsplash.com/photo-1452587925148-ce544e77e70d?w=400'
    'Artes & Oficios' = 'https://images.unsplash.com/photo-1513519245088-0e12902e35ca?w=400'
    'Artesao Cearense' = 'https://images.unsplash.com/photo-1460661419201-fd4cecdf8a8b?w=400'
    'Kit Festa Show' = 'https://images.unsplash.com/photo-1530103862676-de8c9debad1d?w=400'
    'Festa Junina & Cia' = 'https://images.unsplash.com/photo-1516450360452-9312f5e86fc7?w=400'
    'Festas & Eventos' = 'https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=400'
    'Kit Celebracao' = 'https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?w=400'
    'Party Store' = 'https://images.unsplash.com/photo-1511795409834-ef04bbd61622?w=400'
    'Cervejaria Artesanal' = 'https://images.unsplash.com/photo-1532634922-8fe0b757fb13?w=400'
    'Sucos da Terra' = 'https://images.unsplash.com/photo-1622597467836-f3285f2131b8?w=400'
    'Bebidas & Cia' = 'https://images.unsplash.com/photo-1544145945-f90425340c7e?w=400'
    'Destilaria Cearense' = 'https://images.unsplash.com/photo-1470337458703-46ad1756a187?w=400'
    'Drink Artesanal' = 'https://images.unsplash.com/photo-1551024709-8f23befc6f87?w=400'
}

# Replace product ImageUrls: ('name', 'desc', '', price) -> ('name', 'desc', 'url', price)
$productCount = 0
foreach ($name in $productImages.Keys) {
    $escaped = $name.Replace("'", "''")
    $url = $productImages[$name]
    # Pattern: ('ProductName', 'Description', '', price, 'StoreId')
    $old = "'$escaped', '"
    $idx = $content.IndexOf($old)
    while ($idx -ge 0) {
        # Find the third single quote pair (empty ImageUrl)
        $descStart = $idx + $old.Length
        $descEnd = $content.IndexOf("'", $descStart)
        $afterDesc = $content.Substring($descEnd + 1, [Math]::Min(5, $content.Length - $descEnd - 1))
        if ($afterDesc.StartsWith(", '")) {
            # This is the pattern: ...'Description', '', price...
            $emptySlot = $descEnd + 1
            $content = $content.Substring(0, $emptySlot + 1) + $url + $content.Substring($emptySlot + 1)
            $productCount++
            break
        }
        $idx = $content.IndexOf($old, $descEnd)
    }
}

Write-Host "Fixed $productCount product image URLs"

# Verify
$pointCount = ([regex]::Matches($content, "POINT\(")).Count
$unsplashCount = ([regex]::Matches($content, "images\.unsplash\.com")).Count
Write-Host "Total POINT values: $pointCount"
Write-Host "Total Unsplash URLs: $unsplashCount"

# Check first few POINTs
$points = [regex]::Matches($content, "POINT\([^)]+\)")
Write-Host "Sample POINTs after swap:"
for ($i = 1; $i -lt [Math]::Min(6, $points.Count); $i++) {
    Write-Host "  $($points[$i].Value)"
}

Set-Content -Path "C:\Users\vanil\source\repos\IOrderNew\scripts\seed-fortaleza.sql" -Value $content -Encoding utf8 -NoNewline
Write-Host "File saved!"
