﻿// <auto-generated />
using GraphQlResolver.StarWarsV4.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GraphQlResolver.Migrations
{
    [DbContext(typeof(StarWarsContext))]
    [Migration("20190909221709_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("GraphQlResolver.StarWarsV4.Domain.Film", b =>
                {
                    b.Property<int>("EpisodeId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("EpisodeId");

                    b.ToTable("Films");

                    b.HasData(
                        new
                        {
                            EpisodeId = 1,
                            Title = "A New Hope"
                        });
                });

            modelBuilder.Entity("GraphQlResolver.StarWarsV4.Domain.FilmCharacter", b =>
                {
                    b.Property<int>("EpisodeId");

                    b.Property<int>("PersonId");

                    b.HasKey("EpisodeId", "PersonId");

                    b.HasIndex("PersonId");

                    b.ToTable("FilmCharacter");

                    b.HasData(
                        new
                        {
                            EpisodeId = 1,
                            PersonId = 1
                        },
                        new
                        {
                            EpisodeId = 1,
                            PersonId = 2
                        },
                        new
                        {
                            EpisodeId = 1,
                            PersonId = 3
                        });
                });

            modelBuilder.Entity("GraphQlResolver.StarWarsV4.Domain.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Characters");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Luke Skywalker"
                        },
                        new
                        {
                            Id = 2,
                            Name = "C-3PO"
                        },
                        new
                        {
                            Id = 3,
                            Name = "R2-D2"
                        });
                });

            modelBuilder.Entity("GraphQlResolver.StarWarsV4.Domain.FilmCharacter", b =>
                {
                    b.HasOne("GraphQlResolver.StarWarsV4.Domain.Film", "Film")
                        .WithMany("FilmCharacters")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GraphQlResolver.StarWarsV4.Domain.Person", "Character")
                        .WithMany("FilmCharacters")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}