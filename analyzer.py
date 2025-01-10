from errors import ERRORS
from typing import Tuple

class Analyzer:
    operations = ['+', '-', '*', '/', '^', '%']

    DIGITS = [str(i) for i in range(10)]

    STATE_TYPE = ['start', 'object', 'name', 'int', 'num', 'op']

    def __init__(self):
        self.errors: dict[int, int]
        self.text: str

    def parse(self, text: str) -> bool:
        self.text = text
        self.errors = {}
        state = 'start'
        open_brackets = 0
        for index in range(len(self.text)):
            if self.text[index] in ' \t':
                if state in ['name', 'int', 'num']:
                    state = 'object'
            elif self.text[index] in self.DIGITS:
                state = self.check_digit(state, index)
            elif self.text[index] in self.operations:
                state = self.check_op(state, index)
            elif self.text[index] == '.':
                state = self.check_dot(state, index)
            elif self.text[index] == '(':
                state = self.check_open_bracket(state, index)
                open_brackets += 1
            elif self.text[index] == ')':
                (state, open_brackets) = self.check_close_bracket(state, index, open_brackets)
            elif self.text[index].isalpha() or self.text[index] == '_':
                state = self.check_letter(state, index)
            else:
                self.errors[index] = 2
        if state == 'op':
            self.errors[len(text)] = 1
        if open_brackets != 0:
            self.errors[len(text)] = 9 + (open_brackets < 0)
        return not any(self.errors.values())

    def check_letter(self, state: str, index: int) -> str:
        if state in ['int', 'num', 'object']:
            self.errors[index] = 11
        return 'name'


    def check_close_bracket(self, state: str, index: int, open_brackets: int) -> Tuple[str, int]:
        if state == 'op':
            self.errors[index] = 7
        if state == 'start':
            self.errors[index] = 8
        if open_brackets == 0:
            self.errors[index] = 11
        else:
            open_brackets -= 1
        return 'object', open_brackets

    def check_open_bracket(self, state: str, index: int) -> str:
        if state in ['object', 'int', 'num']:
            self.errors[index] = 6
        return 'start'

    def check_dot(self, state: str, index: int) -> str:
        if state == 'int':
            return 'num'
        if state == 'start':
            self.errors[index] = 0
            return 'num'
        self.errors[index] = 5
        return 'num'

    def check_op(self, state: str, index: int) -> str:
        if state == 'start':
            if self.text[index] == '-':
                return 'int'
            self.errors[index] = 0
        if state == 'op':
            self.errors[index] = 4
        return 'op'

    def check_digit(self, state: str, index: int) -> str:
        if state == 'object':
            self.errors[index] = 3
        return 'int' if state in ['start', 'op', 'object'] else state
