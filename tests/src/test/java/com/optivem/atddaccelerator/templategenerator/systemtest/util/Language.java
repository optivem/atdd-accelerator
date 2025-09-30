package com.optivem.atddaccelerator.templategenerator.systemtest.util;

public enum Language {
    NONE("none"),
    JAVA("java"),
    DOTNET("dotnet"),
    TYPESCRIPT("typescript");

    private final String value;

    Language(String value) {
        this.value = value;
    }

    public String getValue() {
        return value;
    }

    public static Language[] getAll() {
        return new Language[] { JAVA, DOTNET, TYPESCRIPT };
    }
}