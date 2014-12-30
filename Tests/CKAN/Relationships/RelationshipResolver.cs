﻿using System;
using System.Collections.Generic;
using CKAN;
using Version = CKAN.Version;
using NUnit.Framework;

namespace Tests.CKAN.Relationships
{
    [TestFixture]
    public class RelationshipResolverTests
    {
        private Registry registry;
        private RelationshipResolverOptions options;
        private RandomModuleGenerator generator;

        [SetUp]
        public void Setup()
        {
            registry = Registry.Empty();
            options = RelationshipResolver.DefaultOpts();
            generator = new RandomModuleGenerator(new Random(0451));
            //Sanity checker means even incorrect RelationshipResolver logic was passing
            options.without_enforce_consistency = true;
        }

        [Test]
        public void Constructor_WithoutModules_AlwaysReturns()
        {
            registry = Registry.Empty();
            options = RelationshipResolver.DefaultOpts();
            Assert.DoesNotThrow(() => new RelationshipResolver(new List<string>(),
                options,
                registry,
                null));
        }

        [Test]
        public void Constructor_WithConflictingModules_Throws()
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        public void Constructor_WithConflictingModulesVersion_Throws()
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, version=mod_a.version.ToString()}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "0.5")]
        [TestCase("1.0", "1.0")]
        public void Constructor_WithConflictingModulesVersionMin_Throws(string ver, string conf_min)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, min_version=conf_min}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0")]
        [TestCase("1.0", "1.0")]
        public void Constructor_WithConflictingModulesVersionMax_Throws(string ver, string conf_max)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, max_version=conf_max}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "0.5", "2.0")]
        [TestCase("1.0", "1.0", "2.0")]
        [TestCase("1.0", "0.5", "1.0")]
        public void Constructor_WithConflictingModulesVersionMinMax_Throws(string ver, string conf_min, string conf_max)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, min_version=conf_min, max_version=conf_max}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "0.5")]
        [TestCase("1.0", "2.0")]
        public void Constructor_WithNonConflictingModulesVersion_DoesNotThrows(string ver, string conf)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, version=conf}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.DoesNotThrow(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0")]
        public void Constructor_WithConflictingModulesVersionMin_DoesNotThrows(string ver, string conf_min)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, min_version="2.0"}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.DoesNotThrow(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("3.0", "2.0")]
        public void Constructor_WithConflictingModulesVersionMax_DoesNotThrows(string ver, string conf_max)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, max_version=conf_max}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.DoesNotThrow(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0", "3.0")]
        [TestCase("4.0", "2.0", "3.0")]
        public void Constructor_WithConflictingModulesVersionMinMax_DoesNotThrows(string ver, string conf_min, string conf_max)
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule(version: new Version(ver));
            var mod_b = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier, min_version=conf_min, max_version=conf_max}
            });

            list.Add(mod_a.identifier);
            list.Add(mod_b.identifier);
            AddToRegistry(mod_a, mod_b);

            Assert.DoesNotThrow(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        public void Constructor_WithMultipleModulesProviding_Throws()
        {
            options.without_toomanyprovides_kraken = false;

            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            var mod_b = generator.GeneratorRandomModule(provides: new List<string>
            {
                mod_a.identifier
            });
            var mod_c = generator.GeneratorRandomModule(provides: new List<string>
            {
                mod_a.identifier
            });
            var mod_d = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=mod_a.identifier}
            });

            list.Add(mod_d.identifier);
            AddToRegistry(mod_b, mod_c, mod_d);
            Assert.Throws<TooManyModsProvideKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        public void Constructor_WithMissingModules_Throws()
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            list.Add(mod_a.identifier);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        // Right now our RR always returns the modules it was provided. However
        // if we've already got the same version(s) installed, it should be able to
        // return a list *without* them. This isn't a hard error at the moment,
        // since ModuleInstaller.InstallList will ignore already installed mods, but
        // it would be nice to have. Discussed a little in GH #521.
        [Test][Category("TODO")][Explicit]
        public void ModList_WithInstalledModules_DoesNotContainThem()
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            list.Add(mod_a.identifier);
            AddToRegistry(mod_a);
            registry.Installed().Add(mod_a.identifier, mod_a.version);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.IsEmpty(relationship_resolver.ModList());
        }

        [Test]
        public void ModList_WithInstalledModulesSugested_DoesNotContainThem()
        {
            options.with_all_suggests = true;
            var list = new List<string>();
            var sugested = generator.GeneratorRandomModule();
            var sugester = generator.GeneratorRandomModule(sugests: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = sugested.identifier}
            });

            list.Add(sugester.identifier);
            AddToRegistry(sugester, sugested);
            registry.Installed().Add(sugested.identifier, sugested.version);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.Contains(relationship_resolver.ModList(), sugested);
        }

        [Test]
        public void ModList_WithSugestedModulesThatWouldConflict_DoesNotContainThem()
        {
            options.with_all_suggests = true;
            var list = new List<string>();
            var sugested = generator.GeneratorRandomModule();
            var mod = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = sugested.identifier}
            });
            var sugester = generator.GeneratorRandomModule(sugests: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = sugested.identifier}
            });

            list.Add(sugester.identifier);
            list.Add(mod.identifier);
            AddToRegistry(sugester, sugested, mod);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.DoesNotContain(relationship_resolver.ModList(), sugested);
        }

        [Test]
        public void Constructor_WithConflictingModulesInDependancies_ThrowUnderDefaultSettings()
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule();
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier}
            });
            var conflicts_with_dependant = generator.GeneratorRandomModule(conflicts: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name=dependant.identifier}
            });


            list.Add(depender.identifier);
            list.Add(conflicts_with_dependant.identifier);
            AddToRegistry(depender, dependant, conflicts_with_dependant);

            Assert.Throws<InconsistentKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));
        }

        [Test]
        public void Constructor_WithSuggests_HasSugestedInModlist()
        {
            options.with_all_suggests = true;
            var list = new List<string>();
            var sugested = generator.GeneratorRandomModule();
            var sugester = generator.GeneratorRandomModule(sugests: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = sugested.identifier}
            });

            list.Add(sugester.identifier);
            AddToRegistry(sugester, sugested);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.Contains(relationship_resolver.ModList(), sugested);
        }

        [Test]
        public void Constructor_ProvidesSatisfyDependencies()
        {
            var list = new List<string>();
            var mod_a = generator.GeneratorRandomModule();
            var mod_b = generator.GeneratorRandomModule(provides: new List<string>
            {
                mod_a.identifier
            });
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = mod_a.identifier}
            });
            list.Add(depender.identifier);
            AddToRegistry(mod_b, depender);
            var relationship_resolver = new RelationshipResolver(list, options, registry, null);

            CollectionAssert.AreEquivalent(relationship_resolver.ModList(), new List<CkanModule>
            {
                mod_b,
                depender
            });

        }


        [Test]
        public void Constructor_WithMissingDependants_Throws()
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule();
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier}
            });
            list.Add(depender.identifier);
            registry.AddAvailable(depender);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0")]
        [TestCase("1.0", "0.2")]
        [TestCase("0", "0.2")]
        [TestCase("1.0", "0")]
        public void Constructor_WithMissingDependantsVersion_Throws(string ver, string dep)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, version = dep}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            AddToRegistry(depender, dependant);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0")]
        public void Constructor_WithMissingDependantsVersionMin_Throws(string ver, string dep_min)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, min_version = dep_min}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            AddToRegistry(depender, dependant);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "0.5")]
        public void Constructor_WithMissingDependantsVersionMax_Throws(string ver, string dep_max)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, max_version = dep_max}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            AddToRegistry(depender, dependant);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "2.0", "3.0")]
        [TestCase("4.0", "2.0", "3.0")]
        public void Constructor_WithMissingDependantsVersionMinMax_Throws(string ver, string dep_min, string dep_max)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, min_version = dep_max}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            AddToRegistry(depender, dependant);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("1.0", "1.0", "2.0")]
        [TestCase("1.0", "1.0", "0.5")]//what to do if a mod is present twice with the same version ?
        public void Constructor_WithDependantVersion_ChooseCorrectly(string ver, string dep, string other)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var other_dependant = generator.GeneratorRandomModule(identifier: dependant.identifier, version: new Version(other));

            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, version = dep}
            });

            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            list.Add(other_dependant.identifier);
            AddToRegistry(depender, dependant, other_dependant);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.AreEquivalent(relationship_resolver.ModList(), new List<CkanModule>
            {
                dependant,
                depender
            });
            
        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("2.0", "1.0", "0.5")]
        [TestCase("2.0", "1.0", "1.5")]
        [TestCase("2.0", "2.0", "0.5")]
        public void Constructor_WithDependantVersionMin_ChooseCorrectly(string ver, string dep_min, string other)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var other_dependant = generator.GeneratorRandomModule(identifier: dependant.identifier, version: new Version(other));

            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, min_version = dep_min}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            list.Add(other_dependant.identifier);
            AddToRegistry(depender, dependant, other_dependant);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.AreEquivalent(relationship_resolver.ModList(), new List<CkanModule>
            {
                dependant,
                depender
            });

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("2.0", "2.0", "0.5")]
        [TestCase("2.0", "3.0", "0.5")]
        [TestCase("2.0", "3.0", "4.0")]
        public void Constructor_WithDependantVersionMax_ChooseCorrectly(string ver, string dep_max, string other)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var other_dependant = generator.GeneratorRandomModule(identifier: dependant.identifier, version: new Version(other));

            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, max_version = dep_max}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            list.Add(other_dependant.identifier);
            AddToRegistry(depender, dependant, other_dependant);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.AreEquivalent(relationship_resolver.ModList(), new List<CkanModule>
            {
                dependant,
                depender
            });

        }

        [Test]
        [Category("Version")]
        [Explicit("Versions relationships not implemented")]
        [TestCase("2.0", "1.0", "3.0", "0.5")]
        [TestCase("2.0", "1.0", "3.0", "1.5")]
        [TestCase("2.0", "1.0", "3.0", "3.5")]
        public void Constructor_WithDependantVersionMinMax_ChooseCorrectly(string ver, string dep_min, string dep_max, string other)
        {
            var list = new List<string>();
            var dependant = generator.GeneratorRandomModule(version: new Version(ver));
            var other_dependant = generator.GeneratorRandomModule(identifier: dependant.identifier, version: new Version(other));

            var depender = generator.GeneratorRandomModule(depends: new List<RelationshipDescriptor>
            {
                new RelationshipDescriptor {name = dependant.identifier, min_version = dep_min, max_version = dep_max}
            });
            list.Add(depender.identifier);
            list.Add(dependant.identifier);
            list.Add(other_dependant.identifier);
            AddToRegistry(depender, dependant, other_dependant);

            var relationship_resolver = new RelationshipResolver(list, options, registry, null);
            CollectionAssert.AreEquivalent(relationship_resolver.ModList(), new List<CkanModule>
            {
                dependant,
                depender
            });

        }

        [Test]
        public void Constructor_WithRegistryThatHasRequiredModuleRemoved_Throws()
        {
            var list = new List<string>();
            var mod = generator.GeneratorRandomModule();
            mod.ksp_version = new KSPVersion("0.10");
            list.Add(mod.identifier);
            registry.AddAvailable(mod);
            registry.RemoveAvailable(mod);

            Assert.Throws<ModuleNotFoundKraken>(() => new RelationshipResolver(
                list,
                options,
                registry,
                null));

        }



        private void AddToRegistry(params CkanModule[] modules)
        {
            foreach (var module in modules)
            {
                registry.AddAvailable(module);
            }
        }
    }
}
