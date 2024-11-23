Log("Lua скрипт запущен!")

if CreateSprite and Vector3 and AttachToParent and ScaleToFitCamera then
    local imagePath = "Mods/test/assets/test.png"
    local position = Vector3(0, 0, 0) -- Центр сцены с изменением по оси Z
    local sprite = CreateSprite(imagePath, position)
    
    if sprite then
        Log("Спрайт успешно создан!")
        
        -- Привязка спрайта к камере "Main Camera"
        AttachToParent(sprite, "Main Camera")
        
        -- Масштабирование спрайта под размеры камеры
        ScaleToFitCamera(sprite, "Main Camera")

        -- Устанавливаем позицию спрайта с нужной координатой Z (например, 10)
        local newPosition = Vector3(830, 112, 10) -- Пример с Z = 10
        SetObjectPosition(sprite, newPosition)
    else
        Log("Ошибка при создании спрайта.")
    end
else
    Log("CreateSprite, Vector3, AttachToParent или ScaleToFitCamera не зарегистрированы!")
end
