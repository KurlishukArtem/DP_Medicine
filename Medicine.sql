-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1:3306
-- Время создания: Июн 17 2025 г., 02:29
-- Версия сервера: 5.7.39-log
-- Версия PHP: 8.1.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `Medicine`
--

-- --------------------------------------------------------

--
-- Структура таблицы `appointments`
--

CREATE TABLE `appointments` (
  `appointment_id` int(11) NOT NULL,
  `patient_id` int(11) DEFAULT NULL,
  `employee_id` int(11) DEFAULT NULL,
  `service_id` int(11) DEFAULT NULL,
  `room_id` int(11) DEFAULT NULL,
  `appointment_date` date NOT NULL,
  `start_time` time NOT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'scheduled',
  `notes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `appointments`
--

INSERT INTO `appointments` (`appointment_id`, `patient_id`, `employee_id`, `service_id`, `room_id`, `appointment_date`, `start_time`, `status`, `notes`, `created_at`) VALUES
(5, 4, 6, 2, 501, '2025-06-18', '16:30:00', 'scheduled', 'asd', '2025-06-17 04:11:28'),
(6, 1, 6, 1, 501, '2025-06-18', '08:30:00', 'scheduled', 'asd', '2025-06-17 04:26:45');

-- --------------------------------------------------------

--
-- Структура таблицы `employees`
--

CREATE TABLE `employees` (
  `employee_id` int(11) NOT NULL,
  `first_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `last_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `middle_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `position` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `specialization` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_date` date DEFAULT NULL,
  `gender` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone_number` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hire_date` date NOT NULL,
  `address` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `login` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `is_active` tinyint(1) DEFAULT '1',
  `rooms` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `employees`
--

INSERT INTO `employees` (`employee_id`, `first_name`, `last_name`, `middle_name`, `position`, `specialization`, `birth_date`, `gender`, `phone_number`, `email`, `hire_date`, `address`, `login`, `password_hash`, `is_active`, `rooms`) VALUES
(5, 'фыв', 'ФЫВ', 'фыар', 'фыа', 'фыа', '2000-06-13', 'M', '85015', '8451', '2025-06-01', 'Cool', 'Col', 'temp123', 1, 0),
(6, 'Ержанн', 'Ержанов', 'Ержанович', 'Врач', 'классная', '2000-06-20', 'M', '88005553535', 'ya.hbuk@yandex.ru', '2025-06-04', 'siojnv', 'hat', '222', 1, 501),
(7, 'Александр', 'Александров', 'Александрович', 'Администратор', 'gvad', '2000-06-24', 'М', '88005553535', 'cool@gmail.ru', '2025-05-02', 'Сибирская', 'Alex', 'Alex', 1, 165),
(8, 'Ксения', 'Суханова', 'Чеевна', 'программист', 'художник(австрийский)', '2000-06-13', 'M', '8028', 'ksen@yandes.ry', '2025-06-12', 'asf', 'Ksen', '555', 1, 0);

-- --------------------------------------------------------

--
-- Структура таблицы `medical_records`
--

CREATE TABLE `medical_records` (
  `record_id` int(11) NOT NULL,
  `patient_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `appointment_id` int(11) DEFAULT NULL,
  `record_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `diagnosis` text COLLATE utf8mb4_unicode_ci,
  `treatment` text COLLATE utf8mb4_unicode_ci,
  `prescription` text COLLATE utf8mb4_unicode_ci,
  `recommendations` text COLLATE utf8mb4_unicode_ci,
  `status` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `medical_tests`
--

CREATE TABLE `medical_tests` (
  `test_id` int(11) NOT NULL,
  `test_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `preparation` text COLLATE utf8mb4_unicode_ci,
  `normal_values` text COLLATE utf8mb4_unicode_ci,
  `price` decimal(10,2) DEFAULT NULL,
  `category` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `medical_tests`
--

INSERT INTO `medical_tests` (`test_id`, `test_name`, `description`, `preparation`, `normal_values`, `price`, `category`) VALUES
(2, 'ihjb', '651', 'iasudn', '51', '500.00', 'uhasjd'),
(3, 'фы', 'ф', 'фыа', 'фа', '591.00', 'авфыв'),
(4, '1', '1', '1', '1', '1.00', '1');

-- --------------------------------------------------------

--
-- Структура таблицы `medications`
--

CREATE TABLE `medications` (
  `medication_id` int(11) NOT NULL,
  `name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci,
  `manufacturer` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dosage_form` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `dosage` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `quantity_in_stock` int(11) DEFAULT '0',
  `minimum_stock_level` int(11) DEFAULT '5',
  `price` decimal(10,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `medications`
--

INSERT INTO `medications` (`medication_id`, `name`, `description`, `manufacturer`, `dosage_form`, `dosage`, `quantity_in_stock`, `minimum_stock_level`, `price`) VALUES
(1, 'adf', 'sadgf', 'fdhga', 'Таблетки', '2-3 ложки на 0.25 литра', 0, 5, '390.00'),
(2, 'флоргексидин', 'кул', 'фыв', 'Капсулы', 'ф25', 0, 5, '862.00'),
(3, '1', 'йц', '1', 'Капсулы', '213', 0, 5, '123.00'),
(4, '123', '', '123', 'Капсулы', '851', 0, 5, '684.00'),
(5, 'Сооl\'', '85941', 'oin', 'Таблетки', '5mg', 65, 5, '500.00');

-- --------------------------------------------------------

--
-- Структура таблицы `patients`
--

CREATE TABLE `patients` (
  `patient_id` int(11) NOT NULL,
  `first_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `last_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `middle_name` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `birth_date` date NOT NULL,
  `gender` char(1) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `phone_number` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `passport_series` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `passport_number` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `snils` varchar(14) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `policy_number` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `registration_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `notes` text COLLATE utf8mb4_unicode_ci,
  `login` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `patients`
--

INSERT INTO `patients` (`patient_id`, `first_name`, `last_name`, `middle_name`, `birth_date`, `gender`, `phone_number`, `email`, `address`, `passport_series`, `passport_number`, `snils`, `policy_number`, `registration_date`, `notes`, `login`, `password_hash`) VALUES
(1, 'Иванов', 'Артём', 'Иванович', '2000-05-14', 'М', '88005553535', 'saf', 'asf', '8005556666', '2518', '36541358', '88005553535', '2025-06-02 11:09:36', 'asf', 'Cool', '123'),
(4, 'Александ', 'Александров', 'Александрович', '2025-06-11', 'М', '88005553535', 'ya.erro2018@yandex.ru', 'khn', '8658561', '15245326', '25605625', '6165851', '2025-06-02 00:00:00', '', 'Kim', '8a33abde321f58694c8e0b9910e93fdd7223c1bb6b4048845474e107880d0eb7');

-- --------------------------------------------------------

--
-- Структура таблицы `payments`
--

CREATE TABLE `payments` (
  `payment_id` int(11) NOT NULL,
  `appointment_id` int(11) DEFAULT NULL,
  `patient_id` int(11) NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `payment_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `payment_method` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'completed'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `prescriptions`
--

CREATE TABLE `prescriptions` (
  `prescription_id` int(11) NOT NULL,
  `record_id` int(11) NOT NULL,
  `medication_id` int(11) NOT NULL,
  `dosage` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `frequency` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `duration` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `instructions` text COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Структура таблицы `schedules`
--

CREATE TABLE `schedules` (
  `schedule_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `day_of_week` int(11) NOT NULL,
  `start_time` time NOT NULL,
  `end_time` time NOT NULL,
  `room_id` int(11) DEFAULT NULL,
  `is_working_day` tinyint(1) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `schedules`
--

INSERT INTO `schedules` (`schedule_id`, `employee_id`, `day_of_week`, `start_time`, `end_time`, `room_id`, `is_working_day`) VALUES
(2, 6, 5, '15:00:00', '16:30:00', 1, 1),
(5, 7, 3, '09:00:00', '09:30:00', 3, 1),
(6, 7, 3, '10:15:45', '10:35:45', 3, 1),
(27, 8, 1, '08:30:00', '09:00:00', 1, 1),
(28, 8, 1, '16:30:00', '17:00:00', 1, 1),
(29, 8, 1, '16:00:00', '16:30:00', 1, 1),
(30, 8, 1, '15:30:00', '16:00:00', 1, 1),
(31, 8, 1, '15:00:00', '15:30:00', 1, 1),
(32, 8, 1, '14:30:00', '15:00:00', 1, 1),
(33, 8, 1, '14:00:00', '14:30:00', 1, 1),
(34, 8, 1, '13:30:00', '14:00:00', 1, 1),
(35, 8, 1, '17:00:00', '17:30:00', 1, 1),
(36, 8, 1, '13:00:00', '13:30:00', 1, 1),
(37, 8, 1, '12:00:00', '12:30:00', 1, 1),
(38, 8, 1, '11:30:00', '12:00:00', 1, 1),
(39, 8, 1, '11:00:00', '11:30:00', 1, 1),
(40, 8, 1, '10:30:00', '11:00:00', 1, 1),
(41, 8, 1, '10:00:00', '10:30:00', 1, 1),
(42, 8, 1, '09:30:00', '10:00:00', 1, 1),
(43, 8, 1, '09:00:00', '09:30:00', 1, 1),
(44, 8, 1, '12:30:00', '13:00:00', 1, 1),
(45, 8, 1, '17:30:00', '18:00:00', 1, 1),
(46, 8, 2, '08:30:00', '09:00:00', 1, 1),
(47, 8, 2, '16:30:00', '17:00:00', 1, 1),
(48, 8, 2, '16:00:00', '16:30:00', 1, 1),
(49, 8, 2, '15:30:00', '16:00:00', 1, 1),
(50, 8, 2, '15:00:00', '15:30:00', 1, 1),
(51, 8, 2, '14:30:00', '15:00:00', 1, 1),
(52, 8, 2, '14:00:00', '14:30:00', 1, 1),
(53, 8, 2, '13:30:00', '14:00:00', 1, 1),
(54, 8, 2, '17:00:00', '17:30:00', 1, 1),
(55, 8, 2, '13:00:00', '13:30:00', 1, 1),
(56, 8, 2, '12:00:00', '12:30:00', 1, 1),
(57, 8, 2, '11:30:00', '12:00:00', 1, 1),
(58, 8, 2, '11:00:00', '11:30:00', 1, 1),
(59, 8, 2, '10:30:00', '11:00:00', 1, 1),
(60, 8, 2, '10:00:00', '10:30:00', 1, 1),
(61, 8, 2, '09:30:00', '10:00:00', 1, 1),
(62, 8, 2, '09:00:00', '09:30:00', 1, 1),
(63, 8, 2, '12:30:00', '13:00:00', 1, 1),
(64, 8, 2, '17:30:00', '18:00:00', 1, 1),
(65, 6, 3, '08:30:00', '09:00:00', 1, 1),
(66, 6, 3, '16:30:00', '17:00:00', 1, 1),
(67, 6, 3, '16:00:00', '16:30:00', 1, 1),
(68, 6, 3, '15:30:00', '16:00:00', 1, 1),
(69, 6, 3, '15:00:00', '15:30:00', 1, 1),
(70, 6, 3, '14:30:00', '15:00:00', 1, 1),
(71, 6, 3, '14:00:00', '14:30:00', 1, 1),
(72, 6, 3, '13:30:00', '14:00:00', 1, 1),
(73, 6, 3, '17:00:00', '17:30:00', 1, 1),
(74, 6, 3, '13:00:00', '13:30:00', 1, 1),
(75, 6, 3, '12:00:00', '12:30:00', 1, 1),
(76, 6, 3, '11:30:00', '12:00:00', 1, 1),
(77, 6, 3, '11:00:00', '11:30:00', 1, 1),
(78, 6, 3, '10:30:00', '11:00:00', 1, 1),
(79, 6, 3, '10:00:00', '10:30:00', 1, 1),
(80, 6, 3, '09:30:00', '10:00:00', 1, 1),
(81, 6, 3, '09:00:00', '09:30:00', 1, 1),
(82, 6, 3, '12:30:00', '13:00:00', 1, 1),
(83, 6, 3, '17:30:00', '18:00:00', 1, 1);

-- --------------------------------------------------------

--
-- Структура таблицы `services`
--

CREATE TABLE `services` (
  `service_id` int(11) NOT NULL,
  `service_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `category` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT '1'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Дамп данных таблицы `services`
--

INSERT INTO `services` (`service_id`, `service_name`, `description`, `price`, `category`, `is_active`) VALUES
(1, 'asd', 'asd', '500.00', 'травмы', 1),
(2, 'фыв', 'фыв', '999.00', 'фыв', 1),
(3, 'фыв', 'фыа', '0.00', 'Консультация', 1),
(4, 'фыв', 'фыв', '0.00', 'Лечение', 1);

-- --------------------------------------------------------

--
-- Структура таблицы `test_results`
--

CREATE TABLE `test_results` (
  `result_id` int(11) NOT NULL,
  `test_id` int(11) NOT NULL,
  `patient_id` int(11) NOT NULL,
  `employee_id` int(11) NOT NULL,
  `appointment_id` int(11) DEFAULT NULL,
  `result_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `result_value` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `comments` text COLLATE utf8mb4_unicode_ci
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `appointments`
--
ALTER TABLE `appointments`
  ADD PRIMARY KEY (`appointment_id`),
  ADD KEY `appointments_ibfk_3` (`service_id`),
  ADD KEY `appointments_ibfk_1` (`patient_id`),
  ADD KEY `appointments_ibfk_2` (`employee_id`);

--
-- Индексы таблицы `employees`
--
ALTER TABLE `employees`
  ADD PRIMARY KEY (`employee_id`),
  ADD UNIQUE KEY `login` (`login`);

--
-- Индексы таблицы `medical_records`
--
ALTER TABLE `medical_records`
  ADD PRIMARY KEY (`record_id`),
  ADD KEY `medical_records_ibfk_3` (`appointment_id`);

--
-- Индексы таблицы `medical_tests`
--
ALTER TABLE `medical_tests`
  ADD PRIMARY KEY (`test_id`);

--
-- Индексы таблицы `medications`
--
ALTER TABLE `medications`
  ADD PRIMARY KEY (`medication_id`);

--
-- Индексы таблицы `patients`
--
ALTER TABLE `patients`
  ADD PRIMARY KEY (`patient_id`);

--
-- Индексы таблицы `payments`
--
ALTER TABLE `payments`
  ADD PRIMARY KEY (`payment_id`),
  ADD KEY `payments_ibfk_1` (`appointment_id`);

--
-- Индексы таблицы `prescriptions`
--
ALTER TABLE `prescriptions`
  ADD PRIMARY KEY (`prescription_id`),
  ADD KEY `medication_id` (`medication_id`),
  ADD KEY `prescriptions_ibfk_1` (`record_id`);

--
-- Индексы таблицы `schedules`
--
ALTER TABLE `schedules`
  ADD PRIMARY KEY (`schedule_id`),
  ADD KEY `schedules_ibfk_1` (`employee_id`);

--
-- Индексы таблицы `services`
--
ALTER TABLE `services`
  ADD PRIMARY KEY (`service_id`);

--
-- Индексы таблицы `test_results`
--
ALTER TABLE `test_results`
  ADD PRIMARY KEY (`result_id`),
  ADD KEY `test_results_ibfk_2_idx` (`appointment_id`),
  ADD KEY `test_results_ibfk_1` (`test_id`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `appointments`
--
ALTER TABLE `appointments`
  MODIFY `appointment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;

--
-- AUTO_INCREMENT для таблицы `employees`
--
ALTER TABLE `employees`
  MODIFY `employee_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT для таблицы `medical_records`
--
ALTER TABLE `medical_records`
  MODIFY `record_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `medical_tests`
--
ALTER TABLE `medical_tests`
  MODIFY `test_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `medications`
--
ALTER TABLE `medications`
  MODIFY `medication_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT для таблицы `patients`
--
ALTER TABLE `patients`
  MODIFY `patient_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `payments`
--
ALTER TABLE `payments`
  MODIFY `payment_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблицы `prescriptions`
--
ALTER TABLE `prescriptions`
  MODIFY `prescription_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT для таблицы `schedules`
--
ALTER TABLE `schedules`
  MODIFY `schedule_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=84;

--
-- AUTO_INCREMENT для таблицы `services`
--
ALTER TABLE `services`
  MODIFY `service_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT для таблицы `test_results`
--
ALTER TABLE `test_results`
  MODIFY `result_id` int(11) NOT NULL AUTO_INCREMENT;

--
-- Ограничения внешнего ключа сохраненных таблиц
--

--
-- Ограничения внешнего ключа таблицы `appointments`
--
ALTER TABLE `appointments`
  ADD CONSTRAINT `appointments_ibfk_1` FOREIGN KEY (`patient_id`) REFERENCES `patients` (`patient_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `appointments_ibfk_2` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `appointments_ibfk_3` FOREIGN KEY (`service_id`) REFERENCES `services` (`service_id`);

--
-- Ограничения внешнего ключа таблицы `medical_records`
--
ALTER TABLE `medical_records`
  ADD CONSTRAINT `medical_records_ibfk_3` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`appointment_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `payments`
--
ALTER TABLE `payments`
  ADD CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`appointment_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `prescriptions`
--
ALTER TABLE `prescriptions`
  ADD CONSTRAINT `prescriptions_ibfk_1` FOREIGN KEY (`record_id`) REFERENCES `medical_records` (`record_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `prescriptions_ibfk_2` FOREIGN KEY (`medication_id`) REFERENCES `medications` (`medication_id`);

--
-- Ограничения внешнего ключа таблицы `schedules`
--
ALTER TABLE `schedules`
  ADD CONSTRAINT `schedules_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employees` (`employee_id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Ограничения внешнего ключа таблицы `test_results`
--
ALTER TABLE `test_results`
  ADD CONSTRAINT `test_results_ibfk_1` FOREIGN KEY (`test_id`) REFERENCES `medical_tests` (`test_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  ADD CONSTRAINT `test_results_ibfk_2` FOREIGN KEY (`appointment_id`) REFERENCES `appointments` (`appointment_id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
