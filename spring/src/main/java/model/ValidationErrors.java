package model;

import java.util.ArrayList;
import java.util.List;

public class ValidationErrors {
    private List<ValidationError> errors = new ArrayList<>();

    public void addError(ValidationError error) {
        this.errors.add(error);
    }

    public List<ValidationError> getErrors() {
        return errors;
    }
}
