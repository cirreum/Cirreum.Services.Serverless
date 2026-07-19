# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.50] - 2026-07-19

### Updated

- Updated NuGet packages.

## [1.0.49] - 2026-07-07

### Removed

- Removed the internal `EmptyTransportPublisher` / `IDistributedTransportPublisher<>` registration from `AddCoreServices`. That transport-publisher seam was dissolved in `Cirreum.Messaging.Distributed 1.2.0` — the no-op type no longer exists, so the registration was a compile error once this package re-pinned to 1.2.0. No effect on serverless hosts: the registration resolved to a no-op that nothing consumed (outbound distribution now runs through the Conductor bridge).

### Updated

- Updated NuGet packages.

## [1.0.46] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.45] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.44] - 2026-07-04

### Updated

- Updated NuGet packages.

## [1.0.43] - 2026-05-10

### Updated

- Updated NuGet packages.

## [1.0.42] - 2026-05-07

### Updated

- Updated NuGet packages.

## [1.0.41] - 2026-05-01

### Updated

- Updated NuGet packages.
