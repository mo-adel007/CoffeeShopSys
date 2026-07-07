-- phpMyAdmin SQL Dump
-- version 4.7.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 30, 2017 at 09:42 PM
-- Server version: 10.1.22-MariaDB
-- PHP Version: 7.1.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `corner`
--

-- --------------------------------------------------------

--
-- Table structure for table `all_monthlyexpenses`
--

CREATE TABLE `all_monthlyexpenses` (
  `id` int(11) NOT NULL,
  `expens_name` varchar(45) DEFAULT NULL,
  `Price` double DEFAULT NULL,
  `Date_time` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `bill`
--

CREATE TABLE `bill` (
  `id_bill` int(11) NOT NULL,
  `pro_name` varchar(50) DEFAULT NULL,
  `pro_price` double DEFAULT NULL,
  `quantity` int(11) DEFAULT NULL,
  `tot_price` double DEFAULT NULL,
  `store` int(11) DEFAULT NULL,
  `IDproduct` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `close_day`
--

CREATE TABLE `close_day` (
  `id` int(11) NOT NULL,
  `processT` varchar(50) DEFAULT NULL,
  `price` double DEFAULT NULL,
  `reason` mediumtext,
  `proName` mediumtext,
  `quantity` int(11) DEFAULT NULL,
  `ReasonOfT` mediumtext,
  `PersonT` varchar(50) DEFAULT NULL,
  `UserN` mediumtext,
  `Dtime` datetime DEFAULT NULL,
  `Userid` int(11) DEFAULT NULL,
  `ShiftNumber` int(11) DEFAULT NULL,
  `whoPerson_add` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `close_shift`
--

CREATE TABLE `close_shift` (
  `id` int(11) NOT NULL,
  `processT` varchar(50) DEFAULT NULL,
  `price` double DEFAULT NULL,
  `reason` mediumtext,
  `proName` mediumtext,
  `quantity` int(11) DEFAULT NULL,
  `ReasonOfT` mediumtext,
  `PersonTake` mediumtext,
  `UserN` mediumtext,
  `Dtime` datetime DEFAULT NULL,
  `Userid` int(11) DEFAULT NULL,
  `ShiftNumber` int(11) DEFAULT NULL,
  `WhoPersonAdd` mediumtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `day_details`
--

CREATE TABLE `day_details` (
  `id_day` int(11) NOT NULL,
  `DayNum` int(11) DEFAULT NULL,
  `TotalSell` double DEFAULT NULL,
  `TotalBuy` double DEFAULT NULL,
  `ProfitDay` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `login`
--

CREATE TABLE `login` (
  `id` int(11) NOT NULL,
  `username` mediumtext,
  `Pass` mediumtext,
  `time_work` mediumtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `login`
--

INSERT INTO `login` (`id`, `username`, `Pass`, `time_work`) VALUES
(1, 'adel', '1234', 'admin');

-- --------------------------------------------------------

--
-- Table structure for table `monthly_expenses`
--

CREATE TABLE `monthly_expenses` (
  `id_expenses` int(11) NOT NULL,
  `expens_name` mediumtext,
  `Price` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `month_details`
--

CREATE TABLE `month_details` (
  `id_Month` int(11) NOT NULL,
  `MonthNumber` int(11) DEFAULT NULL,
  `TotalSell` double DEFAULT NULL,
  `TotalBuy` double DEFAULT NULL,
  `ProfitDay` double DEFAULT NULL,
  `TotalMonthlyExpenses` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `product`
--

CREATE TABLE `product` (
  `Product_id` int(11) NOT NULL,
  `Product_name` mediumtext,
  `Price` double DEFAULT NULL,
  `ProductType_id` int(11) DEFAULT NULL,
  `Store` int(11) DEFAULT NULL,
  `MakeAButton` tinyint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `product`
--

INSERT INTO `product` (`Product_id`, `Product_name`, `Price`, `ProductType_id`, `Store`, `MakeAButton`) VALUES
(1, 'قهوة تركى', NULL, 1, NULL, NULL),
(2, 'قهوة تركى دوبل', NULL, 1, NULL, NULL),
(3, 'قهوة اسبريسو', NULL, 1, NULL, NULL),
(4, 'قهوة اسبريسو دوبل', NULL, 1, NULL, NULL),
(5, 'قهوة بندق', NULL, 1, NULL, NULL),
(6, 'قهوة بالكريم الايرلندى', NULL, 1, NULL, NULL),
(7, 'قهوة فرنساوى', NULL, 1, NULL, NULL),
(8, 'قهوة فرنساوى دوبل', NULL, 1, NULL, NULL),
(9, 'قهوة كراميل', NULL, 1, NULL, NULL),
(10, 'قهوة لوز', NULL, 1, NULL, NULL),
(11, 'قهوة فستق', NULL, 1, NULL, NULL),
(12, 'قهوة شيكولاته', NULL, 1, NULL, NULL),
(13, 'قهوة فانيليا', NULL, 1, NULL, NULL),
(14, 'قهوة ميكس', NULL, 1, NULL, NULL),
(15, 'قهوة نكهات دوبل', NULL, 1, NULL, NULL),
(16, 'قهوة قرفة', NULL, 1, NULL, NULL),
(17, 'نسكافيه', NULL, 3, NULL, NULL),
(18, 'نسكافيه بلاك', NULL, 3, NULL, NULL),
(19, 'نسكافيه كراميل', NULL, 3, NULL, NULL),
(20, 'نسكافيه جولد', NULL, 3, NULL, NULL),
(21, 'كوفى ميكس', NULL, 3, NULL, NULL),
(22, 'كابتشينو ماكينة', NULL, 3, NULL, NULL),
(23, 'كابتشينو كلاسيك', NULL, 3, NULL, NULL),
(24, 'كابتشينو بندق', NULL, 3, NULL, NULL),
(25, 'كابتشينو فانيليا', NULL, 3, NULL, NULL),
(26, 'كابتشينو موكا', NULL, 3, NULL, NULL),
(27, 'كابتشينو شيكولاته', NULL, 3, NULL, NULL),
(28, 'نسكويك', NULL, 3, NULL, NULL),
(29, 'لاتيه', NULL, 3, NULL, NULL),
(30, 'موكا', NULL, 3, NULL, NULL),
(31, 'هوت شوكلت', NULL, 3, NULL, NULL),
(32, 'شوكو ميلك', NULL, 3, NULL, NULL),
(33, 'ماكياتو', NULL, 3, NULL, NULL),
(34, 'فرابينو بالفانيليا', NULL, 3, NULL, NULL),
(35, 'فرابينو بالكراميل', NULL, 3, NULL, NULL),
(36, 'شاى', NULL, 2, NULL, NULL),
(37, 'شاى لاتيه شيكولاته', NULL, 2, NULL, NULL),
(38, 'شاى تفاح', NULL, 2, NULL, NULL),
(39, 'شاى مانجو', NULL, 2, NULL, NULL),
(40, 'شاى توت', NULL, 2, NULL, NULL),
(41, 'شاى فانيليا', NULL, 2, NULL, NULL),
(42, 'شاى مشمش', NULL, 2, NULL, NULL),
(43, 'شاى مانجو و خوخ', NULL, 2, NULL, NULL),
(44, 'شاى توت برى', NULL, 2, NULL, NULL),
(45, 'شاى توت و فراولة', NULL, 2, NULL, NULL),
(46, 'شاى خوخ', NULL, 2, NULL, NULL),
(47, 'شاى خوخ و فواكه استوائية', NULL, 2, NULL, NULL),
(48, 'شاى فراولة', NULL, 2, NULL, NULL),
(49, 'شاى فراولة و كيوى', NULL, 2, NULL, NULL),
(50, 'شاى فراولة و رمان', NULL, 2, NULL, NULL),
(51, 'شاى توت و رمان', NULL, 2, NULL, NULL),
(52, 'شاى عدنى', NULL, 2, NULL, NULL),
(53, 'شاى ياسمين', NULL, 2, NULL, NULL),
(54, 'شاى اخضر', NULL, 2, NULL, NULL),
(55, 'شاى اخضر بالياسمين', NULL, 2, NULL, NULL),
(56, 'شاى إيرل جراى', NULL, 2, NULL, NULL),
(57, 'شاى بالليمون', NULL, 2, NULL, NULL),
(58, 'ينسون', NULL, 7, NULL, NULL),
(59, 'كركديه', NULL, 7, NULL, NULL),
(60, 'نعناع', NULL, 7, NULL, NULL),
(61, 'نعناع بالكاموميل', NULL, 7, NULL, NULL),
(62, 'جنزبيل', NULL, 7, NULL, NULL),
(63, 'كراويه', NULL, 7, NULL, NULL),
(64, 'ليمون بالجنزبيل', NULL, 7, NULL, NULL),
(65, 'قرفة', NULL, 7, NULL, NULL),
(66, 'تليو', NULL, 7, NULL, NULL),
(67, 'جنزبيل بالقرفة', NULL, 7, NULL, NULL),
(68, 'waffle basic', NULL, 5, NULL, NULL),
(69, 'شاى فواكة برية', NULL, 2, NULL, NULL),
(70, 'شاى خوخ وورد', NULL, 2, NULL, NULL),
(71, 'سادة فاتح', NULL, 4, NULL, NULL),
(72, 'سادة وسط', NULL, 4, NULL, NULL),
(73, 'سادة غامق', NULL, 4, NULL, NULL),
(74, 'سادة محروق', NULL, 4, NULL, NULL),
(75, 'محوج فاتح', NULL, 4, NULL, NULL),
(76, 'محوج وسط', NULL, 4, NULL, NULL),
(77, 'محوج غامق', NULL, 4, NULL, NULL),
(78, 'محوج محروق', NULL, 4, NULL, NULL),
(79, 'كولومبى', NULL, 4, NULL, NULL),
(80, 'حبشى', NULL, 4, NULL, NULL),
(81, 'نسكافية فانيليا', NULL, 3, NULL, NULL),
(82, 'نسكافية بندق', NULL, 3, NULL, NULL),
(83, 'شاى بالنعناع', NULL, 2, NULL, NULL),
(84, 'شاى بالقرنفل', NULL, 2, NULL, NULL),
(85, 'لاتية كراميل', NULL, 3, NULL, NULL),
(86, 'شاى مغربى', NULL, 2, NULL, NULL),
(87, 'قهوة كولومبى', NULL, 1, NULL, NULL),
(88, 'قهوة حبشى', NULL, 1, NULL, NULL),
(89, 'مياة كبير', NULL, 6, NULL, NULL),
(90, 'مياة صغير', NULL, 6, NULL, NULL),
(91, 'كانز كبير', NULL, 6, NULL, NULL),
(92, 'فروتز', NULL, 6, NULL, NULL),
(93, 'لبن', NULL, 6, NULL, NULL),
(94, 'اضافة حليب', NULL, 2, NULL, NULL),
(95, 'نسكافية موكا', NULL, 3, NULL, NULL),
(96, 'قهوة تركى محوج', NULL, 1, NULL, NULL),
(97, 'قهوة تركى محوج دوبل', NULL, 1, NULL, NULL),
(98, 'قهوة بالحليب', NULL, 1, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `product_process`
--

CREATE TABLE `product_process` (
  `id_Process` int(11) NOT NULL,
  `User_Name` mediumtext,
  `Process_type` mediumtext,
  `Product_name` mediumtext,
  `quantity` int(11) DEFAULT NULL,
  `price` double DEFAULT NULL,
  `DateTime` datetime DEFAULT NULL,
  `IdProduct` int(11) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `product_type`
--

CREATE TABLE `product_type` (
  `type_id` int(11) NOT NULL,
  `type_name` mediumtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `product_type`
--

INSERT INTO `product_type` (`type_id`, `type_name`) VALUES
(1, 'قهوة'),
(2, 'شاى'),
(3, 'نسكافيه'),
(4, 'البن'),
(5, 'waffle'),
(6, 'الثلاجة'),
(7, 'أخرى');

-- --------------------------------------------------------

--
-- Table structure for table `safe`
--

CREATE TABLE `safe` (
  `id` int(11) NOT NULL,
  `Type` varchar(50) DEFAULT NULL,
  `price` double DEFAULT NULL,
  `reason` mediumtext,
  `proName` mediumtext,
  `quantity` int(11) DEFAULT NULL,
  `ReasonOfT` mediumtext,
  `PersonTake` mediumtext,
  `UserN` mediumtext,
  `Dtime` datetime DEFAULT NULL,
  `Userid` int(11) DEFAULT NULL,
  `Who_personAdd` mediumtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `shift_details`
--

CREATE TABLE `shift_details` (
  `id_shift` int(11) NOT NULL,
  `ShiftNum` int(11) DEFAULT NULL,
  `TotalSell` double DEFAULT NULL,
  `TotalBuy` double DEFAULT NULL,
  `ProfitShift` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `timing`
--

CREATE TABLE `timing` (
  `Login_Date_time` datetime NOT NULL,
  `User_Name` mediumtext,
  `Logout_date_time` datetime DEFAULT NULL,
  `User_id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `user_shift`
--

CREATE TABLE `user_shift` (
  `id` int(11) NOT NULL,
  `UserName` mediumtext,
  `ShiftNumber` mediumtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `all_monthlyexpenses`
--
ALTER TABLE `all_monthlyexpenses`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `bill`
--
ALTER TABLE `bill`
  ADD PRIMARY KEY (`id_bill`);

--
-- Indexes for table `close_day`
--
ALTER TABLE `close_day`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Day_user_idx` (`Userid`);

--
-- Indexes for table `close_shift`
--
ALTER TABLE `close_shift`
  ADD PRIMARY KEY (`id`),
  ADD KEY `Shift_user_idx` (`Userid`);

--
-- Indexes for table `day_details`
--
ALTER TABLE `day_details`
  ADD PRIMARY KEY (`id_day`);

--
-- Indexes for table `login`
--
ALTER TABLE `login`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `monthly_expenses`
--
ALTER TABLE `monthly_expenses`
  ADD PRIMARY KEY (`id_expenses`);

--
-- Indexes for table `month_details`
--
ALTER TABLE `month_details`
  ADD PRIMARY KEY (`id_Month`);

--
-- Indexes for table `product`
--
ALTER TABLE `product`
  ADD PRIMARY KEY (`Product_id`);

--
-- Indexes for table `product_process`
--
ALTER TABLE `product_process`
  ADD PRIMARY KEY (`id_Process`);

--
-- Indexes for table `product_type`
--
ALTER TABLE `product_type`
  ADD PRIMARY KEY (`type_id`);

--
-- Indexes for table `safe`
--
ALTER TABLE `safe`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `shift_details`
--
ALTER TABLE `shift_details`
  ADD PRIMARY KEY (`id_shift`);

--
-- Indexes for table `timing`
--
ALTER TABLE `timing`
  ADD PRIMARY KEY (`Login_Date_time`,`User_id`);

--
-- Indexes for table `user_shift`
--
ALTER TABLE `user_shift`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `all_monthlyexpenses`
--
ALTER TABLE `all_monthlyexpenses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `bill`
--
ALTER TABLE `bill`
  MODIFY `id_bill` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `close_day`
--
ALTER TABLE `close_day`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `close_shift`
--
ALTER TABLE `close_shift`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `day_details`
--
ALTER TABLE `day_details`
  MODIFY `id_day` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `login`
--
ALTER TABLE `login`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
--
-- AUTO_INCREMENT for table `monthly_expenses`
--
ALTER TABLE `monthly_expenses`
  MODIFY `id_expenses` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `month_details`
--
ALTER TABLE `month_details`
  MODIFY `id_Month` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `product`
--
ALTER TABLE `product`
  MODIFY `Product_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=99;
--
-- AUTO_INCREMENT for table `product_process`
--
ALTER TABLE `product_process`
  MODIFY `id_Process` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `product_type`
--
ALTER TABLE `product_type`
  MODIFY `type_id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
--
-- AUTO_INCREMENT for table `safe`
--
ALTER TABLE `safe`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `shift_details`
--
ALTER TABLE `shift_details`
  MODIFY `id_shift` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `user_shift`
--
ALTER TABLE `user_shift`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `close_day`
--
ALTER TABLE `close_day`
  ADD CONSTRAINT `Day_user` FOREIGN KEY (`Userid`) REFERENCES `login` (`id`) ON DELETE NO ACTION ON UPDATE CASCADE;

--
-- Constraints for table `close_shift`
--
ALTER TABLE `close_shift`
  ADD CONSTRAINT `Shift_user` FOREIGN KEY (`Userid`) REFERENCES `login` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
