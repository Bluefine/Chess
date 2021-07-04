<h1 align="center"> Chess </h1> <br>

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [To-Do](#to-do)
- [Changelog](#changelog)

## Introduction

Chess engine written in C# with the .NET 5.0. The engine is still in development.

## Usage

Chess is not a complete chess program and requires a UCI-compatible graphical user interface (GUI) (e.g. XBoard with PolyGlot, Scid, Cute Chess, eboard, Arena, Sigma Chess, Shredder, Chess Partner or Fritz) in order to be used comfortably.

## Features

* Alpha-beta pruning search
* Transposition table (Zobrist hash)
* Piece-Square tables
* Uci interface (need improvements)
* Iterative deepening

## To-Do

* Move ordering
* Multithreading
* Faster move generation
* Killer moves
* And More...

## Changelog

#### 1.4.0.0 (04.07.2021)
* Added bestscore variable determined from evaluation function
* Changed player representation from string to bool
* Best move got own function and a bit of optimization
* Best move now also check for nodes in depth which was not finished before time
* Transposition get own function and is temporarily disabled cuz there is bug which i need to find (some problems with check-mate search)

#### 1.3.0.0 (27.06.2021)
* Added 'en passant' move
* Added threefold repetition check
* Redesigned MovePiece function
* AlphaBeta Code refactoring

#### 1.2.0.0 (23.06.2021)
* Move ordering by distance to center of the board
* Transposition replacing with depther values (need to be tested if working properly)
* Fixed castling
* Fixed IsCheck and IsCheckMate detection
* Code refactoring

#### 1.1.0.0 (19.06.2021)
* Removed name from piece model, no need to use whole string one char is enough and will be faster
* Redesigned piece update function
* Redesigned whole "safemove" function which was used to check if move is legal, now it's named "ValidMove"
* Added "CastleQueenSide" for Move model to detect castle side
* Improved search algorithm to don't perform unnecessary calculations
* And others smaller changes

#### 1.0.0.0 (15.06.2021)
* First release
