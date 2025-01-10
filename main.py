from errors import ERRORS
from analyzer import Analyzer
import sys
# from terminal import render

if __name__ == '__main__':
    text = ' '.join(sys.argv[1:])

    analyzer = Analyzer()
    success = analyzer.parse(text)

    if success:
        print(f'Text "{text}" successfully parsed')
    else:
        print('ERRORS:')
        for position, error in analyzer.errors.items():
            print(f'\tError #{error} - Index {position}: {ERRORS[error]}')
            print(f'\t{text[:position]}' + '\u0332'.join(list(text[position:])) + '\u0332')
