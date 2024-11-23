-- Получаем доступ к функции логирования
Log("Мод активирован!")

-- Функция для воспроизведения звука
function PlaySound()
    -- Путь к звуковому файлу в папке мода
    local soundPath = "./Mods/Example/test.mp3"

    -- Проверка, существует ли файл
    if FileExists(soundPath) then
        -- Воспроизводим звук
        PlayAudio(soundPath)
        Log("Звук воспроизведён!")
    else
        Log("Ошибка: Файл не найден!")
    end
end

-- Регистрация события нажатия клавиши G
function Update()
    if IsKeyPressed("G") then
        PlaySound()
    end
end

