# Hypercube Quicksort Algorithm

1. В цикле по итерациям от 1 до размерности гиперкуба:
   1. Выбираем опорный элемент
   2. "Ведущие" для своего подгиперкуба процессоры рассылают свои опорные элементы
   3. Каждый процесс разделяет свой массив данных на два фрагмента: low (<= опорному элементу) и high (>= опорному элементу)
   4. Представим ранг процессора как число в двоичной системе исчисления, отправим "соседу" по гиперкубу один из подмассивов из [low, hi], в зависимости от четности i-того бита в ранге (0 -> отправить hi, 1-> отправить low)
   5. Каждый процессор выполняет переупорядочивание и слияние своей и полученной части
2. Корневой процесс собирает кусочки данных со всех процессов, в i-том кусочке все элементы меньше, чем в i+1 => все элементы упорядочены