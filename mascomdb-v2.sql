-- phpMyAdmin SQL Dump
-- version 5.0.4
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1
-- Généré le : lun. 20 juin 2022 à 11:09
-- Version du serveur :  10.4.17-MariaDB
-- Version de PHP : 7.4.13

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `mascomdb`
--
CREATE DATABASE IF NOT EXISTS `mascomdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `mascomdb`;

-- --------------------------------------------------------

--
-- Structure de la table `admins`
--

CREATE TABLE `admins` (
  `Id` int(11) NOT NULL,
  `LastName` varchar(128) DEFAULT NULL,
  `FirstName` varchar(128) DEFAULT NULL,
  `Email` longtext DEFAULT NULL,
  `UserName` varchar(128) NOT NULL,
  `PassworHash` longtext NOT NULL,
  `DateJoined` datetime(6) NOT NULL,
  `LastLogin` datetime(6) NOT NULL,
  `IsActive` tinyint(1) NOT NULL,
  `IsStaff` tinyint(1) NOT NULL,
  `IsSuperAdmin` tinyint(1) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Structure de la table `filemessageheaders`
--

CREATE TABLE `filemessageheaders` (
  `Id` int(11) NOT NULL,
  `Extension` longtext DEFAULT NULL,
  `FileName` longtext DEFAULT NULL,
  `Delivered` tinyint(1) NOT NULL,
  `DeliveryId` longtext DEFAULT NULL,
  `FileFetchUrl` longtext DEFAULT NULL,
  `MessageType` int(11) NOT NULL,
  `FileType` int(11) NOT NULL,
  `RecipientId` int(11) NOT NULL,
  `SenderId` int(11) NOT NULL,
  `CreationDate` datetime(6) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `filemessageheaders`
--

INSERT INTO `filemessageheaders` (`Id`, `Extension`, `FileName`, `Delivered`, `DeliveryId`, `FileFetchUrl`, `MessageType`, `FileType`, `RecipientId`, `SenderId`, `CreationDate`) VALUES
(1, '.vsix', 'AddAnyFile', 0, 'b22ec2', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(2, '.vsix', 'AddAnyFile', 0, 'a7bee7', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(3, '.vsix', 'AddAnyFile', 0, 'af620c', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(4, '.docx', 'Doc2', 0, 'bf0483', NULL, 1, 5, -1200, -1, '0001-01-01 00:00:00.000000'),
(5, '.vsix', 'AddAnyFile', 0, '7c64da', NULL, 1, 5, -1200, -1, '0001-01-01 00:00:00.000000'),
(6, '.vsix', 'AddAnyFile', 0, 'c799dc', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(7, '.vsix', 'AddAnyFile', 0, '531623', NULL, 1, 5, -1200, -1, '0001-01-01 00:00:00.000000'),
(8, '.docx', 'Doc2', 0, 'c835ce', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(9, '.vsix', 'AddAnyFile', 0, 'f14c9e', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(10, '.vsix', 'AddAnyFile', 0, '3b3ca4', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(11, '.vsix', 'AddAnyFile', 0, '47a4b8', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(12, '.vsix', 'AddAnyFile', 0, '2e0930', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(13, '.mp3', 'Little Mix  No More Sad Songs (Official Video) ft. Machine Gun Kelly (REACTION)', 0, '2e54d3', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(14, '.mp3', 'Little Mix  No More Sad Songs (Official Video) ft. Machine Gun Kelly (REACTION)', 0, 'ef582d', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(15, '.vsix', 'AddAnyFile', 0, 'dd72af', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(16, '.vsix', 'AddAnyFile', 0, 'f1c35d', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(17, '.docx', 'Doc2', 0, '273d1f', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(18, '.mp3', 'Little Mix  No More Sad Songs (Official Video) ft. Machine Gun Kelly (REACTION)', 0, '97c8f1', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(19, '.7z', 'Docker Desktop Installer', 0, '6bcfd9', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(20, '.blend2', 'New wave chenille', 0, '0bde9c', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(21, '.blend2', 'New wave chenille', 0, 'aaf1cb', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(22, '.mp3', 'Work', 0, '92ada0', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(23, '.7z', 'Docker Desktop Installer', 0, 'bf14f5', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(24, '.mp3', 'Metanoia-Stellar Fountain Presents _ Winter 2019', 0, 'c67300', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(25, '.vsix', 'AddAnyFile', 0, 'cc4438', NULL, 1, 5, -1200, -1, '0001-01-01 00:00:00.000000'),
(26, '.7z', 'WhatsAppSetup', 0, 'e0da3c', NULL, 1, 5, -1200, -1, '0001-01-01 00:00:00.000000'),
(27, '.7z', 'WhatsAppSetup', 0, 'a40534', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(28, '.mp3', 'Dancing In The Dark', 0, 'eff19f', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000'),
(29, '.vsix', 'AddAnyFile', 0, 'af1288', NULL, 1, 5, -1, -1, '0001-01-01 00:00:00.000000');

-- --------------------------------------------------------

--
-- Structure de la table `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `UserName` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `LastName` varchar(128) NOT NULL,
  `PasswordHash` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `users`
--

INSERT INTO `users` (`Id`, `UserName`, `Name`, `LastName`, `PasswordHash`) VALUES
(-1200, 'Baba', 'Manzourou Alao rrrr..Gbaaa', 'Manzourou', '1234'),
(-220, 'Sam', 'Oala Samson', 'Samson', '1234'),
(-1, 'Eric', 'Hotegni', 'Eric', 'ggg');

-- --------------------------------------------------------

--
-- Structure de la table `usertosessionsmappings`
--

CREATE TABLE `usertosessionsmappings` (
  `Id` int(11) NOT NULL,
  `UserId` int(11) NOT NULL,
  `P2PSid` longtext DEFAULT NULL,
  `GroupSid` longtext DEFAULT NULL,
  `FileDeliverySid` longtext DEFAULT NULL,
  `FileHeadersDeliverySid` longtext DEFAULT NULL,
  `VideoCallSid` longtext DEFAULT NULL,
  `VideoConfSid` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Déchargement des données de la table `usertosessionsmappings`
--

INSERT INTO `usertosessionsmappings` (`Id`, `UserId`, `P2PSid`, `GroupSid`, `FileDeliverySid`, `FileHeadersDeliverySid`, `VideoCallSid`, `VideoConfSid`) VALUES
(1, -1, '5ee19f0dda104e0093192a273518f90e', NULL, 'bb42908852f34d3e821df8b81143936e', '64d34ca6fd2b4bd68ca38df27457d0da', NULL, NULL),
(2, -1200, '9d17bf83aa464fc3944e1a73af3964ec', NULL, 'cca84b5d881849f1ae959c4cf1155468', '69cf045bd8fc4d99a3b5ef32f12ba076', NULL, NULL);

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `admins`
--
ALTER TABLE `admins`
  ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `filemessageheaders`
--
ALTER TABLE `filemessageheaders`
  ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- Index pour la table `usertosessionsmappings`
--
ALTER TABLE `usertosessionsmappings`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `admins`
--
ALTER TABLE `admins`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `filemessageheaders`
--
ALTER TABLE `filemessageheaders`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT pour la table `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `usertosessionsmappings`
--
ALTER TABLE `usertosessionsmappings`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
